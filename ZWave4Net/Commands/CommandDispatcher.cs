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
        private readonly List<PendingCommand> _pendingCommands = new List<PendingCommand>();

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
            var request = _pendingCommands.FirstOrDefault(element => element.Message == e.Message && element.Response == null);
            if (request != null)
            {
                _pendingCommands.Remove(request);
                request.CompletionSource.SetResult(null);
                return;
            }
        }

        private void OnEventReceived(object sender, EventMessageEventArgs e)
        {
            if (e.Message.NodeID != CommandClass.Node.NodeID)
                return;
            if (e.Message.Command.ClassID != CommandClass.ClassID)
                return;

            var request = _pendingCommands.FirstOrDefault(element => Convert.ToByte(element.Response) == e.Message.Command.CommandID);
            if (request != null)
            {
                _pendingCommands.Remove(request);
                request.CompletionSource.SetResult(e.Message.Command);
                return;
            }
            CommandClass.HandleEvent(e.Message.Command);
        }

        public async Task<Command> Send(Command command, Enum response)
        {
            var message = new Message(CommandClass.Node.NodeID, command);
            var completionSource = new TaskCompletionSource<Command>();

            var pendingCommand = new PendingCommand(message, completionSource, response);

            _pendingCommands.Add(pendingCommand);
            try
            {
                await Channel.Send(message).ConfigureAwait(false);
                return await completionSource.Task.Run(ResponseTimeout).ConfigureAwait(false);
            }
            catch (TimeoutException)
            {
                _pendingCommands.Remove(pendingCommand);
                throw;
            }
        }

        public Task Post(Command command)
        {
            return Send(command, null);
        }

        class PendingCommand
        {
            public readonly Message Message;
            public readonly TaskCompletionSource<Command> CompletionSource;
            public readonly Enum Response;

            public PendingCommand(Message message, TaskCompletionSource<Command> completionSource, Enum response = null)
            {
                Message = message;
                CompletionSource = completionSource;
                Response = response;
            }
        }
    }
}
