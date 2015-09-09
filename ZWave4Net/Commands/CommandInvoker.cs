using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave4Net.Communication;

namespace ZWave4Net.Commands
{
    class CommandInvoker : ICommandInvoker
    {
        private readonly List<Tuple<Command, TaskCompletionSource<Command>>> _pendingCommands = new List<Tuple<Command, TaskCompletionSource<Command>>>();

        public readonly CommandClass CommandClass;
        public TimeSpan ResponseTimeout = TimeSpan.FromSeconds(1);

        public CommandInvoker(CommandClass commandClass)
        {
            CommandClass = commandClass;
            Channel.EventReceived += OnEventReceived;
        }

        private IMessageChannel Channel
        {
            get { return CommandClass.Node.Driver.Channel; }
        }

        private void OnEventReceived(object sender, EventMessageEventArgs e)
        {
            if (e.Message.NodeID != CommandClass.Node.NodeID)
                return;
            if (e.Message.Command.ClassID != CommandClass.ClassID)
                return;

            var request = _pendingCommands.FirstOrDefault(element => CommandClass.IsCorrelated(element.Item1, e.Message.Command));
            if (request != null)
            {
                _pendingCommands.Remove(request);
                request.Item2.SetResult(e.Message.Command);
                return;
            }

            CommandClass.HandleEvent(e.Message.Command);
        }

        public async Task<Command> Invoke(Command command)
        {
            var payload = new List<byte>();
            payload.Add(CommandClass.Node.NodeID);
            payload.AddRange(command.Serialize());

            var completionSource = new TaskCompletionSource<Command>();
            var tuple = Tuple.Create(command, completionSource);
            _pendingCommands.Add(tuple);

            try
            {
                await Channel.Send(new Message(MessageType.Request, Function.SendData, payload.ToArray()));
                return await completionSource.Task.Run(ResponseTimeout);
            }
            catch (TimeoutException)
            {
                _pendingCommands.Remove(tuple);
                throw;
            }

        }
    }
}
