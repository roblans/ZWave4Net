using System;
using System.Collections.Generic;
using System.Linq;
using ZWave.Channel;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class AlarmReport : NodeReport
    {
        public AlarmType Type { get; private set; }
        public byte Level { get; private set; }
        public AlarmDetailType Detail { get; private set; }
        public byte Unknown { get; private set; }

        internal AlarmReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));

            if (payload.Length < 2)
                throw new ReponseFormatException($"The response was not in the expected format. Payload{BitConverter.ToString(payload)}");

            Type = (AlarmType)payload[0];
            Level = payload[1];
            if (payload.Length > 2)
            {
                Unknown = payload[2];
            }
            if (payload.Length > 5)
            {
                Detail = (AlarmDetailType)payload[5];
            }
        }

        public override string ToString()
        {
            return $"Type:{Type}, Level:{Level}, Detail:{Detail}, Unknown:{Unknown}";
        }
    }
}
