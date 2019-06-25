using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave.Channel;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class SensorAlarmSupportedReport : NodeReport
    {
        public readonly AlarmType[] Types;

        internal SensorAlarmSupportedReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));

            Types = payload.Select(p => (AlarmType)p).ToArray();
        }

        public override string ToString()
        {
            return $"Types:{Types}";
        }
    }
}
