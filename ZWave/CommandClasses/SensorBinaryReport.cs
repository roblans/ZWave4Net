using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class SensorBinaryReport : NodeReport
    {
        public readonly byte Value;

        internal SensorBinaryReport(Node node, byte[] payload) : base(node)
        {
            Value = payload[0];
        }

        public override string ToString()
        {
            return $"Value:{Value}";
        }
    }
}
