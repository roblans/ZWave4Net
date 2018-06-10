using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Channel.Protocol
{
    class ControllerFunctionCompleted : Message, IMessageWithPayload
    {
        readonly byte[] payload;

        public byte[] Payload { get { return payload; } }

        public ControllerFunctionCompleted(Function function, byte[] payload)
            : base(FrameHeader.SOF, MessageType.Response, function)
        {
            this.payload = payload;
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
