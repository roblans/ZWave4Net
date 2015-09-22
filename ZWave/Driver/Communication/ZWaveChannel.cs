using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Driver.Communication.Protocol;

namespace ZWave.Driver.Communication
{
    public class ZWaveChannel : IZWaveChannel
    {
        private Task _portReadTask;
        private Task _processEventsTask;
        private Task _transmitTask;
        private readonly BlockingCollection<NodeEvent> _eventQueue = new BlockingCollection<NodeEvent>();
        private readonly BlockingCollection<Message> _transmitQueue = new BlockingCollection<Message>();
        private readonly BlockingCollection<Message> _responseQueue = new BlockingCollection<Message>();

        public readonly SerialPort Port;
        public TextWriter Log;
        public TimeSpan ResponseTimeout = TimeSpan.FromSeconds(2);
        public event EventHandler<NodeEventArgs> NodeEventReceived;

        public ZWaveChannel(string portName)
        {
            Port = new SerialPort(portName);
            _processEventsTask = new Task(() => ProcessQueue(_eventQueue, OnNodeEventReceived));
            _transmitTask = new Task(() => ProcessQueue(_transmitQueue, OnTransmit));
            _portReadTask = new Task(() => ReadPort(Port));
        }

        private void LogMessage(string message)
        {
            if (Log != null)
            {
                Log.WriteLine(message);
            }
        }

        private async void ReadPort(SerialPort port)
        {
            while (true)
            {
                try
                {
                    // wait for message received (blocking)
                    var message = await Message.Read(port.InputStream).ConfigureAwait(false);
                    LogMessage(string.Format($"Received: {message}"));

                    // ignore ACK, no processing of ACK needed
                    if (message == Message.ACK)
                        continue;

                    // is it a eventmessage from a node?
                    if (message is NodeEvent)
                    {
                        // yes, so add to eventqueue
                        _eventQueue.Add((NodeEvent)message);
                        // send ACK to controller
                        _transmitQueue.Add(Message.ACK);
                        // we're done
                        continue;
                    }

                    // not a event, so it's a response to a request
                    _responseQueue.Add(message);
                    // send ACK to controller
                    _transmitQueue.Add(Message.ACK);
                }
                catch (ChecksumException ex)
                {
                    LogMessage(string.Format($"Exception: {ex.Message}"));
                    _transmitQueue.Add(Message.NAK);
                }
                catch (UnknownFrameException ex)
                {
                    LogMessage(string.Format($"Exception: {ex.Message}"));
                }
                catch (IOException)
                {
                    // port closed, we're done so return
                    return;
                }
            }
        }

        private void ProcessQueue<T>(BlockingCollection<T> queue, Action<T> process) where T : Message
        {
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

        private void OnNodeEventReceived(NodeEvent nodeEvent)
        {
            var handler = NodeEventReceived;
            if (handler != null)
            {
                handler(this, new NodeEventArgs(nodeEvent.NodeID, nodeEvent.Command));
            }
        }

        private void OnTransmit(Message message)
        {
            message.Write(Port.OutputStream).ConfigureAwait(false);
            LogMessage(string.Format($"Transmitted: {message}"));
        }

        private async Task<Message> Receive()
        {
            var result = await Task.Run(() =>
            {
                var message = default(Message);
                _responseQueue.TryTake(out message, ResponseTimeout);
                return message;
            });

            if (result == null)
                throw new TimeoutException();
            if (result == Message.NAK)
                throw new NakResponseException();
            if (result == Message.CAN)
                throw new CanResponseException();

            return result;
        }

        private async Task<Message> WaitForResponse(Func<Message, bool> predicate)
        {
            while (true)
            {
                var result = await Task.Run(() =>
                {
                    var message = default(Message);
                    _responseQueue.TryTake(out message, ResponseTimeout);
                    return message;
                });

                if (result == null)
                    throw new TimeoutException();
                if (result == Message.NAK)
                    throw new NakResponseException();
                if (result == Message.CAN)
                    throw new CanResponseException();

                if (predicate(result))
                {
                    return result;
                }
            }
        }

        public void Open()
        {
            Port.Open();

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

        private async Task<Byte[]> SendCommand(byte nodeID, Command command, byte? responseCommandID = null)
        {
            var attempt = 1;
            while (true)
            {
                try
                {
                    if (responseCommandID != null)
                    {
                        var completionSource = new TaskCompletionSource<Command>();
                        var cancellationTokenSource = new CancellationTokenSource();
                        cancellationTokenSource.CancelAfter(ResponseTimeout);
                        cancellationTokenSource.Token.Register(() => completionSource.TrySetCanceled(), useSynchronizationContext: false);

                        EventHandler<NodeEventArgs> onNodeEventReceived = (_, e) =>
                        {
                            if (e.NodeID == nodeID && e.Command.ClassID == command.ClassID && e.Command.CommandID == responseCommandID)
                            {
                                completionSource.SetResult(e.Command);
                            }
                        };

                        var request = new NodeCommand(nodeID, command);
                        _transmitQueue.Add(request);

                        NodeEventReceived += onNodeEventReceived;
                        try
                        {
                            var response = await WaitForResponse((message) =>
                            {
                                return (message is NodeCommandCompleted && ((NodeCommandCompleted)message).CallbackID == request.CallbackID);
                            });

                            try
                            {
                                return (await completionSource.Task).Payload;
                            }
                            catch(TaskCanceledException)
                            {
                                throw new TimeoutException();
                            }
                        }
                        finally
                        {
                            NodeEventReceived -= onNodeEventReceived;
                        }
                    }
                    else
                    {
                        var request = new NodeCommand(nodeID, command);
                        _transmitQueue.Add(request);

                        var response = await WaitForResponse((message) =>
                        {
                            return (message is NodeCommandCompleted && ((NodeCommandCompleted)message).CallbackID == request.CallbackID);
                        });

                        return null;
                    }

                }
                catch (CanResponseException)
                {
                    LogMessage(string.Format($"CAN received. Retrying (attempt: {attempt})"));

                    if (attempt++ > 3)
                        throw;
                }
            }
        }

        private void onNodeEventReceived(object sender, NodeEventArgs e)
        {
            throw new NotImplementedException();
        }

        public async Task<byte[]> Send(Function function, params byte[] payload)
        {
            var attempt = 1;
            while (true)
            {
                try
                {
                    var request = new ControllerFunction(function);
                    _transmitQueue.Add(request);

                    var response = await WaitForResponse((message) =>
                    {
                        return message is ControllerFunctionCompleted;
                    });

                    return ((ControllerFunctionCompleted)response).Payload;
                }
                catch (CanResponseException)
                {
                    LogMessage(string.Format($"CAN received. Retrying (attempt: {attempt})"));

                    if (++attempt > 3)
                        throw;
                }
            }
        }

        public Task Send(byte nodeID, Command command)
        {
            return SendCommand(nodeID, command, null);
        }

        public Task<Byte[]> Send(byte nodeID, Command command, byte responseCommandID)
        {
            return SendCommand(nodeID, command, responseCommandID);
        }
    }
}
