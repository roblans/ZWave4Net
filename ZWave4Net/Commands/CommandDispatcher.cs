using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave4Net.Communication;

namespace ZWave4Net.Commands
{
    class CommandDispatcher : ICommandDispatcher
    {
        private readonly List<Tuple<Message, TaskCompletionSource<Command>, Enum>> _pendingCommands = new List<Tuple<Message, TaskCompletionSource<Command>, Enum>>();

        public readonly CommandClass CommandClass;
        public TimeSpan ResponseTimeout = TimeSpan.FromSeconds(10);

        public CommandDispatcher(CommandClass commandClass)
        {
            CommandClass = commandClass;
            Channel.SendCompleted += OnSendCompleted;
            Channel.EventReceived += OnEventReceived;
        }


        private IMessageChannel Channel
        {
            get { return CommandClass.Node.Driver.Channel; }
        }

        private void OnSendCompleted(object sender, MessageEventArgs e)
        {
            var request = _pendingCommands.FirstOrDefault(element => element.Item1 == e.Message && element.Item3 == null);
            if (request != null)
            {
                _pendingCommands.Remove(request);
                request.Item2.SetResult(null);
                return;
            }
        }

        private void OnEventReceived(object sender, EventMessageEventArgs e)
        {
            if (e.Message.NodeID != CommandClass.Node.NodeID)
                return;
            if (e.Message.Command.ClassID != CommandClass.ClassID)
                return;

            var request = _pendingCommands.FirstOrDefault(element => Convert.ToByte(element.Item3) == e.Message.Command.CommandID);
            if (request != null)
            {
                _pendingCommands.Remove(request);
                request.Item2.SetResult(e.Message.Command);
                return;
            }

            CommandClass.HandleEvent(e.Message.Command);
        }

        public async Task<Command> Send(Command command, Enum replyCommandID)
        {
            var message = new Message(CommandClass.Node.NodeID, command);
            var completionSource = new TaskCompletionSource<Command>();
            var tuple = Tuple.Create(message, completionSource, replyCommandID);
            _pendingCommands.Add(tuple);

            try
            {
                await Channel.Send(message).ConfigureAwait(false);
                return await completionSource.Task.Run(ResponseTimeout).ConfigureAwait(false);
            }
            catch (TimeoutException)
            {
                _pendingCommands.Remove(tuple);
                throw;
            }
        }

        public Task Post(Command command)
        {
            return Send(command, null);
        }
    }
}
