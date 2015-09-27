using System;
using System.Collections.Generic;
using System.Linq;
using ZWave.Channel;

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
            Type = (AlarmType)payload[0];
            Level = payload[1];
            Unknown = payload[2];
            Detail = (AlarmDetailType)payload[5];
        }

        public override string ToString()
        {
            return $"Type:{Type}, Level:{Level}, Detail:{Detail}, Unknown:{Unknown}";
        }
    }
}
