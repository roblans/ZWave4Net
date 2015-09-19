using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net.Communication
{
    interface IMessageChannel
    {
        Task<Message> Send(Message request);
        event EventHandler<MessageEventArgs> SendCompleted;
        event EventHandler<EventMessageEventArgs> EventReceived;
    }

    static partial class Extentions
    {
        public static Task<Message> Send(this IMessageChannel channel, Function function)
        {
            return channel.Send(new Message(MessageType.Request, function));
        }

        public static Task<Message> Send(this IMessageChannel channel, Function function, params byte[] payload)
        {
            return channel.Send(new Message(MessageType.Request, function, payload));
        }

        public static Task<Message> Send(this IMessageChannel channel, Function function, Node node)
        {
            return channel.Send(new Message(MessageType.Request, function, node.NodeID));
        }
    }

}
