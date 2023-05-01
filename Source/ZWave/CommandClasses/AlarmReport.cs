using System;
using System.Collections.Generic;
using System.Linq;
using ZWave.Channel;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class AlarmReport : NodeReport
    {
        public NotificationType Type { get; private set; }
        public byte Level { get; private set; }
        public byte Status { get; private set; }
        public AlarmDetailType Event { get; private set; }
        public byte SourceNodeID { get; private set; }
        public byte[] Params { get; private set; }

        internal AlarmReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));

            if (payload.Length < 2)
                throw new ReponseFormatException($"The response was not in the expected format. Report: {GetType().Name}, Payload: {BitConverter.ToString(payload)}");

            //Version 1
            Type = (NotificationType)payload[0];
            Status = Level = payload[1];

            //Version 2
            if (payload.Length > 5)
            {
                SourceNodeID = payload[2];
                Status = payload[3];
                Type = (NotificationType)payload[4];
                Event = (AlarmDetailType)payload[5];
            }
            else
            {
                SourceNodeID = 0;
                Status = 0;
                Event = AlarmDetailType.None;
            }

            if (payload.Length > 6)
            {
                Params = new byte[payload[6]];
                Buffer.BlockCopy(payload, 7, Params, 0, Params.Length);
            }
            else
                Params = new byte[0];
        }

        public override string ToString()
        {
            return $"Type:{Type}, Level:{Level}, Event:{Event}, SourceID:{SourceNodeID}";
        }
    }
}
