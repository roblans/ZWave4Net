using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Driver.Communication.Protocol
{
    class UnknownMessage : Message
    {
        public readonly byte[] Payload;

        public UnknownMessage(FrameHeader header, MessageType type, Function function, byte[] payload)
            : base(header, type, function)
        {
            Payload = payload;
        }

        public override string ToString()
        {
            return string.Concat(base.ToString(), " ", string.Format($"Payload:{BitConverter.ToString(Payload)} (unknown message!)"));
        }
    }
}
