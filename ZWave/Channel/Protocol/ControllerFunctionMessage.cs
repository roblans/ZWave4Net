using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Channel.Protocol
{
    class ControllerFunctionMessage : Message
    {
        public readonly byte[] Payload;

        public ControllerFunctionMessage(MessageType type, Function function, byte[] payload)
            : base(FrameHeader.SOF, type, function)
        {
            Payload = payload;
        }

        public override string ToString()
        {
            if (Payload != null)
            {
                return string.Concat(base.ToString(), " ", $"Payload:{BitConverter.ToString(Payload)}");
            }
            return base.ToString();
        }
    }
}
