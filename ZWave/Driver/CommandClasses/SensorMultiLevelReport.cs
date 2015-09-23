using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZWave.Driver.Communication;

namespace ZWave.Driver.CommandClasses
{
    public class SensorMultiLevelReport : NodeReport
    {
        public readonly SensorType Type;
        public readonly byte[] Value;

        internal SensorMultiLevelReport(Node node, byte[] payload) : base(node)
        {
            // 4 bytes: 3, 10, 0, 212
            Type = (SensorType)payload[0];
            Value = payload.Skip(1).ToArray();
        }

        public override string ToString()
        {
            return $"Type:{Type}, Value:{BitConverter.ToString(Value)}";
        }
    }
}
