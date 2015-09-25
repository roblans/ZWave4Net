using System;
using System.Collections.Generic;
using System.Linq;
using ZWave.Channel;

namespace ZWave.Controller.CommandClasses
{
    public class BatteryReport : NodeReport
    {
        public readonly byte Value;

        internal BatteryReport(Node node, byte[] payload) : base(node)
        {
            Value = payload[0] == 0xFF ? (byte)0x00 : payload[0];
        }

        public override string ToString()
        {
            return $"Value:{Value}%";
        }
    }
}
