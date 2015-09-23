using System;
using System.Collections.Generic;
using System.Linq;
using ZWave.Driver.Communication;

namespace ZWave.Driver.CommandClasses
{
    public class AlarmReport : NodeReport
    {
        public AlarmType Type { get; private set; }
        public byte Level { get; private set; }
        public AlarmDetailType Detail { get; private set; }
        public byte Unknown { get; private set; }

        internal AlarmReport(Node node, byte[] payload) : base(node)
        {
            if (payload.Length < 4)
            {
                // V1
                Type = (AlarmType)payload[0];
                Level = payload[1];
                Unknown = payload[2];
            }
            else
            {
                // V2
                Level = payload[3];
                Type = (AlarmType)payload[4];
                Detail = (AlarmDetailType)payload[5];
                Unknown = payload[6];
            }
        }

        public override string ToString()
        {
            return $"Type:{Type}, Level:{Level}, Detail:{Detail}, Unknown:{Unknown}";
        }
    }
}
