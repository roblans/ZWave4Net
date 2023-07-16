using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel.Protocol;

namespace ZWave.Channel
{
    public class ZWaveChannel
    {
        private static byte _functionID = 0;
        private readonly SemaphoreSlim _semaphore;
        private Task _portReadTask;
        private Task _processEventsTask;
        private Task _transmitTask;
        private BlockingCollection<Message> _eventQueue;
        private BlockingCollection<Message> _transmitQueue;
        private BlockingCollection<Message> _responseQueue;

        public readonly ISerialPort Port;
        public TextWriter Log { get; set; }
        public TimeSpan ReceiveTimeout = TimeSpan.FromSeconds(2);
        public TimeSpan ResponseTimeout = TimeSpan.FromSeconds(5);
        public event EventHandler<NodeEventArgs> NodeEventReceived;
        public event EventHandler<NodeUpdateEventArgs> NodeUpdateReceived;
        internal event EventHandler<ControllerFunctionMessage> NodesNetworkChangeOccurred;
        public event EventHandler<ErrorEventArgs> Error;
        public event EventHandler Closed;

        public int MaxRetryCount { get; set; } = 3;

        public ZWaveChannel(ISerialPort port)
        {
            if ((Port = port) == null)
                throw new ArgumentNullException(nameof(port));

            _semaphore = new SemaphoreSlim(1, 1);
        }

        public ZWaveChannel(string portName)
             : this(new SerialPort(portName))
        {
        }

#if WINDOWS_UWP
        public ZWaveChannel(ushort vendorId, ushort productId)
             : this(new SerialPort(vendorId, productId))
        {
        }
#endif

        protected virtual void OnError(ErrorEventArgs e)
        {
            LogMessage($"Exception: {e.Error}");
            Error?.Invoke(this, e);
        }

        protected virtual void OnClosed(EventArgs e)
        {
            LogMessage($"Connection closed");
            Closed?.Invoke(this, e);
        }

        private void LogMessage(string message)
        {
            if (Log != null && message != null)
            {
                Log.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd H:mm:ss.fff")} {message}");
            }
        }

        private async void ReadPort(ISerialPort port)
        {
            if (port == null)
                throw new ArgumentNullException(nameof(port));

            while (true)
            {
                try
                {
                    // wait for message received (blocking)
                    var message = await Message.Read(port.InputStream).ConfigureAwait(false);
                    LogMessage($"Received: {message}");

                    // ignore ACK, no processing of ACK needed
                    if (message == Message.ACK)
                        continue;

                    // is it a eventmessage from a node?
                    if (message is NodeEvent)
                    {
                        // yes, so add to eventqueue
                        _eventQueue.Add(message);
                        // send ACK to controller
                        _transmitQueue.Add(Message.ACK);
                        // we're done
                        continue;
                    }

                    // is it a updatemessage from a node?
                    if (message is NodeUpdate)
                    {
                        // yes, so add to eventqueue
                        _eventQueue.Add(message);
                        // send ACK to controller
                        _transmitQueue.Add(Message.ACK);
                        // we're done
                        continue;
                    }

                    // not a event, so it's a response to a request
                    _responseQueue.Add(message);
                    // send ACK to controller
                    _transmitQueue.Add(Message.ACK);

                    if ((message.Function == Function.AddNodeToNetwork || message.Function == Function.RemoveNodeFromNetwork)
                        && message is ControllerFunctionMessage controllerFunctionMessage)
                    {
                        NodesNetworkChangeOccurred?.Invoke(this, controllerFunctionMessage);
                    }
                }
                catch (ChecksumException ex)
                {
                    LogMessage($"Exception: {ex}");
                    _transmitQueue.Add(Message.NAK);
                }
                catch (UnknownFrameException ex)
                {
                    // probably out of sync on the serial port
                    // ToDo: handle gracefully 
                    OnError(new ErrorEventArgs(ex));
                }
                catch (IOException)
                {
                    // port closed, we're done so return
                    OnClosed(EventArgs.Empty);
                    return;
                }
                catch (Exception ex)
                {
                    // just raise error event. don't break reading of serial port
                    OnError(new ErrorEventArgs(ex));
                }
            }
        }

        private void ProcessQueue<T>(BlockingCollection<T> queue, Action<T> process) where T : Message
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));
            if (process == null)
                throw new ArgumentNullException(nameof(process));

            while (!queue.IsCompleted)
            {
                var message = default(Message);
                try
                {
                    message = queue.Take();
                }
                catch (InvalidOperationException)
                {
                }

                if (message != null)
                {
                    process((T)message);
                }
            }
        }

        private void OnNodeMessageReceived(Message message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (message is NodeEvent)
            {
                OnNodeEventReceived((NodeEvent)message);
                return;
            }
            if (message is NodeUpdate)
            {
                OnNodeUpdateReceived((NodeUpdate)message);
                return;
            }
        }

        private void OnNodeEventReceived(NodeEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var handler = NodeEventReceived;
            if (handler != null)
            {
                foreach (var invocation in handler.GetInvocationList().Cast<EventHandler<NodeEventArgs>>())
                {
                    try
                    {
                        invocation(this, new NodeEventArgs(@event.SourceNodeID, @event.Command));
                    }
                    catch (Exception ex)
                    {
                        LogMessage(ex.ToString());
                    }
                }
            }
        }

        private void OnNodeUpdateReceived(NodeUpdate update)
        {
            if (update == null)
                throw new ArgumentNullException(nameof(update));

            var handler = NodeUpdateReceived;
            if (handler != null)
            {
                foreach (var invocation in handler.GetInvocationList().Cast<EventHandler<NodeUpdateEventArgs>>())
                {
                    try
                    {
                        invocation(this, new NodeUpdateEventArgs(update.NodeID));
                    }
                    catch (Exception ex)
                    {
                        LogMessage(ex.ToString());
                    }
                }
            }
        }

        private void OnTransmit(Message message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            message.Write(Port.OutputStream).ConfigureAwait(false);
            LogMessage($"Transmitted: {message}");
        }

        private async Task<Message> WaitForResponse(Func<Message, bool> predicate, CancellationToken cancellationToken)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            while (!cancellationToken.IsCancellationRequested)
            {
                var result = await Task.Run((Func<Message>)(() =>
                {
                    var message = default(Message);
                    _responseQueue.TryTake(out message, (int)ResponseTimeout.TotalMilliseconds, cancellationToken);
                    return message;
                })).ConfigureAwait(false);

                if (result == null)
                    throw new TimeoutException();
                if (result == Message.NAK)
                    throw new NakResponseException();
                if (result == Message.CAN)
                    throw new CanResponseException();
                if (result is NodeCommandCompleted && ((NodeCommandCompleted)result).TransmissionState != TransmissionState.CompleteOk)
                    throw new TransmissionException($"Transmission failure: {((NodeCommandCompleted)result).TransmissionState}.");

                if (predicate(result))
                {
                    return result;
                }
            }

            throw new TaskCanceledException();
        }

        public void Open()
        {
            Port.Open();

            // create tasks, on open or re-open
            _eventQueue = new BlockingCollection<Message>();
            _transmitQueue = new BlockingCollection<Message>();
            _responseQueue = new BlockingCollection<Message>();

            _processEventsTask = new Task(() => ProcessQueue(_eventQueue, OnNodeMessageReceived));
            _transmitTask = new Task(() => ProcessQueue(_transmitQueue, OnTransmit));
            _portReadTask = new Task(() => ReadPort(Port));
            
            // start tasks
            _portReadTask.Start();
            _processEventsTask.Start();
            _transmitTask.Start();

        }

        public void Close()
        {
            Port.Close();

            _eventQueue.CompleteAdding();
            _responseQueue.CompleteAdding();
            _transmitQueue.CompleteAdding();

            _portReadTask.Wait();
            _processEventsTask.Wait();
            _transmitTask.Wait();
        }

        internal async Task SoftReset(CancellationToken cancellationToken =  default)
        {
            // Reset Serial Connection
            _transmitQueue.Add(Message.NAK);
            _transmitQueue.Add(new ControllerFunction(Function.SerialApiSoftReset));
            await Task.Delay(1500, cancellationToken);
        }

        private async Task<Byte[]> Exchange(Func<Task<Byte[]>> func, string message, CancellationToken cancellationToken)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var attempt = 0;
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        return await func().ConfigureAwait(false);
                    }
                    catch (CanResponseException)
                    {
                        if (attempt++ >= MaxRetryCount)
                            throw;

                        LogMessage($"CAN received on: {message}. Retrying attempt: {attempt}");

                        await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken).ConfigureAwait(false);
                    }
                    catch (TransmissionException)
                    {
                        if (attempt++ >= MaxRetryCount)
                            throw;

                        LogMessage($"Transmission failure on: {message}. Retrying attempt: {attempt}");

                        await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken).ConfigureAwait(false);
                    }
                    catch (TimeoutException)
                    {
                        if (attempt++ >= MaxRetryCount)
                            throw;

                        LogMessage($"Timeout on: {message}. Retrying attempt: {attempt}");

                        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            finally
            {
                _semaphore.Release();
            }

            throw new TaskCanceledException();
        }

        private Task<byte[]> Send(Function function, byte[] payload, Func<ControllerFunctionMessage, bool> predicate, CancellationToken cancellationToken = default)
        {
            return Exchange(async () =>
            {
                var request = new ControllerFunction(function, payload);
                _transmitQueue.Add(request);

                var response = await WaitForResponse((message) => predicate((ControllerFunctionMessage)message), cancellationToken).ConfigureAwait(false);

                return ((ControllerFunctionMessage)response).Payload;
            }, $"{function} {(payload != null ? BitConverter.ToString(payload) : string.Empty)}", cancellationToken);
        }

        public Task<byte[]> Send(Function function, params byte[] payload)
        {
            return Send(function, CancellationToken.None, payload);
        }

        public Task<byte[]> Send(Function function, CancellationToken cancellationToken, params byte[] payload)
        {
            return Send(function, payload, (message) =>
            {
                return (message is ControllerFunctionCompleted && ((ControllerFunctionCompleted)message).Function == function);
            }, cancellationToken);
        }

        public Task<byte[]> SendWithFunctionId(Function function, byte[] payload, Func<byte[], bool> predicate = null, CancellationToken cancellationToken = default)
        {
            byte functionId = GetNextFunctionID();
            return Send(function, payload.Concat(new byte[] { functionId }).ToArray(), (message) =>
            {
                if (!(message is ControllerFunctionEvent))
                {
                    return false;
                }

                byte[] responsePayload = message.Payload;
                if (!(message is ControllerFunctionEvent) || responsePayload[0] != functionId)
                {
                    return false;
                }

                if (predicate != null)
                {
                    return predicate(responsePayload.Skip(1).ToArray());
                }

                return true;
            }, cancellationToken);
        }

        public Task Send(byte nodeID, Command command, CancellationToken cancellationToken = default)
        {
            if (nodeID == 0)
                throw new ArgumentOutOfRangeException(nameof(nodeID), nodeID, "nodeID can not be 0");
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            return Exchange(async () =>
            {
                var request = new NodeCommand(nodeID, command);
                _transmitQueue.Add(request);

                await WaitForResponse((message) =>
                {
                    return (message is NodeCommandCompleted && ((NodeCommandCompleted)message).CallbackID == request.CallbackID);
                }, cancellationToken).ConfigureAwait(false);

                return null;
            }, $"NodeID:{nodeID:D3}, Command:{command}", cancellationToken);
        }

        public Task<Byte[]> Send(byte nodeID, Command command, byte responseCommandID, CancellationToken cancellationToken = default)
        {
            return Send(nodeID, command, responseCommandID, null, cancellationToken);
        }

        public Task<Byte[]> Send(byte nodeID, Command command, byte responseCommandID, Func<byte[], bool> payloadValidation, CancellationToken cancellationToken = default)
        {
            if (nodeID == 0)
                throw new ArgumentOutOfRangeException(nameof(nodeID), nodeID, "nodeID can not be 0");
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            return Exchange(async () =>
            {
                var completionSource = new TaskCompletionSource<Command>();

                EventHandler<NodeEventArgs> onNodeEventReceived = (_, e) =>
                {
                    if (e.NodeID == nodeID && e.Command.ClassID == command.ClassID && e.Command.CommandID == responseCommandID)
                    {
                        if (payloadValidation == null || payloadValidation(e.Command.Payload))
                        {
                            // BugFix: 
                            // http://stackoverflow.com/questions/19481964/calling-taskcompletionsource-setresult-in-a-non-blocking-manner
                            Task.Run(() => completionSource.TrySetResult(e.Command));
                        }
                    }
                };

                var request = new NodeCommand(nodeID, command);
                _transmitQueue.Add(request);

                NodeEventReceived += onNodeEventReceived;
                try
                {
                    await WaitForResponse((message) =>
                    {
                        return (message is NodeCommandCompleted && ((NodeCommandCompleted)message).CallbackID == request.CallbackID);
                    }, cancellationToken).ConfigureAwait(false);

                    try
                    {
                        using (var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                        {
                            cancellationTokenSource.CancelAfter(ResponseTimeout);
                            cancellationTokenSource.Token.Register(() => completionSource.TrySetCanceled(), useSynchronizationContext: false);

                            var response = await completionSource.Task.ConfigureAwait(false);
                            return response.Payload;
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        // Rethrow only if the external cancellation token was canceled.
                        //
                        if (cancellationToken.IsCancellationRequested)
                        {
                            throw;
                        }

                        throw new TimeoutException();
                    }
                }
                finally
                {
                    NodeEventReceived -= onNodeEventReceived;
                }
            }, $"NodeID:{nodeID:D3}, Command:[{command}], Reponse:{responseCommandID}", cancellationToken);
        }

        private static byte GetNextFunctionID()
        {
            lock (typeof(ZWaveChannel)) { return _functionID = (byte)((_functionID % 255) + 1); }
        }
    }
}
