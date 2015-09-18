using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net.Communication
{
    class MessageChannel : IMessageChannel
    {
        private Task _receiveTask;
        private Task _transmitTask;
        private Task _portMonitorTask;
        private readonly BlockingCollection<Message> _receiveQueue = new BlockingCollection<Message>();
        private readonly BlockingCollection<Message> _transmitQueue = new BlockingCollection<Message>();
        private readonly List<Tuple<Message, TaskCompletionSource<Message>>> _pendingMessages = new List<Tuple<Message, TaskCompletionSource<Message>>>();

        public event EventHandler<EventMessageEventArgs> EventReceived;

        public TimeSpan ResponseTimeout = TimeSpan.FromSeconds(10);
        public readonly ISerialPort Port;

        public MessageChannel(ISerialPort port)
        {
            Port = port;
        }

        public void Open()
        {
            Port.Open();

            _receiveTask = new Task(() => ProcessQueue(_receiveQueue, OnReceived), TaskCreationOptions.LongRunning);
            _receiveTask.Start();

            _transmitTask = new Task(() => ProcessQueue(_transmitQueue, OnTransmit), TaskCreationOptions.LongRunning);
            _transmitTask.Start();

            _portMonitorTask = new Task(() => ProcessPort(Port), TaskCreationOptions.LongRunning);
            _portMonitorTask.Start();

        }

        public void Close()
        {
            _receiveQueue.CompleteAdding();
            _transmitQueue.CompleteAdding();

            _receiveTask.Wait();
            _transmitTask.Wait();

            Port.Close();
        }

        private async void ProcessPort(ISerialPort port)
        {
            while (true)
            {
                try
                {
                    // ToDo: implement Message.Read(Stream, CancelationToken) to allow graceful termination of this task
                    var message = await Message.Read(port.InputStream).ConfigureAwait(false);
                    _receiveQueue.Add(message);
                }
                catch (ChecksumException ex)
                {
                    Platform.LogMessage(LogLevel.Error, ex.Message);
                    _transmitQueue.Add(Message.NegativeAcknowledgment);
                    throw;
                }
                catch (UnknownFrameException ex)
                {
                    Platform.LogMessage(LogLevel.Error, ex.Message);
                    throw;
                }
                catch (IOException)
                {
                    // port closed, we're done so return
                    return;
                }
            }
        }

        private void ProcessQueue(BlockingCollection<Message> queue, Action<Message> process)
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
                    process(message);
                }
            }
        }

        private void OnReceived(Message response)
        {
            Platform.LogMessage(LogLevel.Debug, string.Format($"Received: {response}"));

            switch (response.Header)
            {
                case FrameHeader.ACK:
                    return;
                case FrameHeader.CAN:
                    // what to do here?
                    return;
                case FrameHeader.NAK:
                    throw new NakResponseException();
                case FrameHeader.SOF:
                    _transmitQueue.Add(Message.Acknowledgment);
                    break;
            }

            var request = _pendingMessages.FirstOrDefault(element => IsComplete(element.Item1, response));
            if (request != null)
            {
                _pendingMessages.Remove(request);
                request.Item2.SetResult(response);
            }

            if (response.Function == Function.ApplicationCommandHandler)
            {
                var eventMessage = EventMessage.Parse(response.Payload);
                OnEventReceived(new EventMessageEventArgs(eventMessage));
            }
        }

        protected virtual void OnEventReceived(EventMessageEventArgs e)
        {
            var handler = EventReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private bool IsComplete(Message request, Message response)
        {
            if (request.Command == null)
            {
                return request.Function == response.Function;
            }
            if (request.Function == Function.SendData)
            {
                if (response.Function == Function.ApplicationCommandHandler && request.NodeID == response.NodeID && request.Command.ClassID == response.Command.ClassID)
                {
                    return true;
                }

            }
            return false;
        }

        private void OnTransmit(Message request)
        {
            Platform.LogMessage(LogLevel.Debug, string.Format($"Transmitting: {request}"));
            request.Write(Port.OutputStream);
        }

        public async Task<Message> Send(Message request)
        {
            var completionSource = new TaskCompletionSource<Message>();

            var tuple = Tuple.Create(request, completionSource);
            _pendingMessages.Add(tuple);
            _transmitQueue.Add(request);

            try
            {
                return await completionSource.Task.Run(ResponseTimeout).ConfigureAwait(false);
            }
            catch (TimeoutException)
            {
                _pendingMessages.Remove(tuple);
                throw;
            }
        }
    }
}
