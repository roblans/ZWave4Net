using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Communication.Protocol
{
    class NodeEvent : Message
    {
        public readonly ReceiveStatus ReceiveStatus;
        public readonly byte NodeID;
        public readonly Command Command;

        public NodeEvent(byte[] payload)
            : base(FrameHeader.SOF, MessageType.Response, Communication.Function.ApplicationCommandHandler)
        {
            ReceiveStatus = (ReceiveStatus)payload[0];
            NodeID = payload[1];
            Command = Command.Parse(payload.Skip(2).ToArray());
        }

        public override string ToString()
        {
            return string.Concat(base.ToString(), " ", $"{ReceiveStatus}, NodeID:{NodeID}, Command:[{Command}]");
        }
    }
}
