using System;
using System.Collections.Generic;
using System.Linq;
using ZWave.Channel;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class NotificationReport : NodeReport
    {
        public NotificationType V1Type { get; protected set; }
        public NotificationType Type { get; protected set; }
        public byte Level { get; protected set; }
        public byte Status { get; protected set; }
        public NotificationState Event { get; protected set; }
        public byte SourceNodeID { get; protected set; }
        public byte[] Params { get; protected set; }
        public byte SequenceNum { get; protected set; }

        internal NotificationReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 7)
                throw new ReponseFormatException($"The response was not in the expected format. Report: {GetType().Name}, Payload: {BitConverter.ToString(payload)}");

            V1Type = (NotificationType)payload[0];
            Level = payload[1];
            SourceNodeID = payload[2];
            Status = payload[3];
            Type = (NotificationType)payload[4];
            Event = (NotificationState)((payload[4] << 8) | payload[5]);
            if (payload[5] == 0x0)
                Event = NotificationState.Idle;
            else if (payload[5] == 0xFE)
                Event = NotificationState.Unknown;
            Params = new byte[payload[6] & 0x1F];
            Buffer.BlockCopy(payload, 7, Params, 0, Params.Length);
            if ((payload[6] & 0x80) == 0x80)
                SequenceNum = payload[payload.Length - 1];
        }

        public override string ToString()
        {
            return $"Type:{Type}, Level:{Level}, Event:{Event}, SourceID:{SourceNodeID}";
        }
    }
}
