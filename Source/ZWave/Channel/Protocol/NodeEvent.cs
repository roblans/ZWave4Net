using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Channel.Protocol
{
    class NodeEvent : Message
    {
        public readonly ReceiveStatus ReceiveStatus;
        public readonly byte NodeID;
        public readonly Command Command;

        public NodeEvent(byte[] payload)
            : base(FrameHeader.SOF, MessageType.Response, Channel.Function.ApplicationCommandHandler)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 2)
                throw new ReponseFormatException($"The response was not in the expected format. NodeEvent, payload: {BitConverter.ToString(payload)}");

            ReceiveStatus = ReceiveStatus.None;

            if ((payload[0] & 0x01) > 0)
                ReceiveStatus |= ReceiveStatus.RoutedBusy;
            if ((payload[0] & 0x02) > 0)
                ReceiveStatus |= ReceiveStatus.LowPower;
            if ((payload[0] & 0x0C) == 0x00)
                ReceiveStatus |= ReceiveStatus.TypeSingle;
            if ((payload[0] & 0x0C) == 0x01)
                ReceiveStatus |= ReceiveStatus.TypeBroad;
            if ((payload[0] & 0x0C) == 0x10)
                ReceiveStatus |= ReceiveStatus.TypeMulti;
            if ((payload[0] & 0x10) > 0)
                ReceiveStatus |= ReceiveStatus.TypeExplore;
            if ((payload[0] & 0x40) > 0)
                ReceiveStatus |= ReceiveStatus.ForeignFrame;

            NodeID = payload[1];
            Command = Command.Parse(payload.Skip(2).ToArray());
        }

        public override string ToString()
        {
            return string.Concat(base.ToString(), " ", $"{ReceiveStatus}, NodeID:{NodeID:D3}, Command:[{Command}]");
        }
    }
}
