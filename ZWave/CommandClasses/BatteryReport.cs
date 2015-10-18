using System;
using System.Collections.Generic;
using System.Linq;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class BatteryReport : NodeReport
    {
        public readonly byte Value;
        public readonly bool IsLow;

        internal BatteryReport(Node node, byte[] payload) : base(node)
        {
            IsLow = payload[0] == 0xFF;
            Value = IsLow ? (byte)0x00 : payload[0];
        }

        public override string ToString()
        {
            return IsLow ? "Low" : $"Value:{Value}%";
        }
    }
}
