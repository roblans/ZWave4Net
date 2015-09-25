using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.Controller.CommandClasses
{
    public class SwitchBinaryReport : NodeReport
    {
        public readonly byte Value;

        internal SwitchBinaryReport(Node node, byte[] payload) : base(node)
        {
            Value = payload[0];
        }

        public override string ToString()
        {
            return $"Value:{Value}";
        }
    }
}
