using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Channel.Protocol
{
    class UnknownMessage : Message, IMessageWithPayload
    {
        readonly byte[] payload;

        public byte[] Payload { get { return payload; } }

        public UnknownMessage(FrameHeader header, MessageType type, Function function, byte[] payload)
            : base(header, type, function)
        {
            this.payload = payload;
        }

        public override string ToString()
        {
            return string.Concat(base.ToString(), " ", $"Payload:{BitConverter.ToString(Payload)}");
        }
    }
}
