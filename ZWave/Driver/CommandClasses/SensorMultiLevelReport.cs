using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Driver.Communication;

namespace ZWave.Driver.CommandClasses
{
    public class SensorMultiLevelReport : NodeReport
    {
        private readonly byte[] _payload;
        public readonly SensorType Type;
        public readonly byte Value;

        internal SensorMultiLevelReport(Node node, byte[] payload) : base(node)
        {
            _payload = payload;
            Type = (SensorType)payload[0];
            // 4 bytes: 3, 10, 0, 212
            Value = payload[0];
        }

        public override string ToString()
        {
            return $"Type:{Type}, {BitConverter.ToString(_payload)}";
        }
    }
}
