using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class SensorAlarmReport : NodeReport
    {
        public readonly byte Source;
        public readonly AlarmType Type;
        public readonly byte Level;

        internal SensorAlarmReport(Node node, byte[] payload) : base(node)
        {
            // 5 bytes: byte 3 and 4 unknown
            Source = payload[0];
            Type = (AlarmType)payload[1];
            Level = payload[2];
        }

        public override string ToString()
        {
            return $"Source:{Source}, Type:{Type}, Level:{Level}";
        }
    }
}
