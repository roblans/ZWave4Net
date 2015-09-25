using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Controller.CommandClasses
{
    public class BasicReport : NodeReport
    {
        public readonly byte Value;

        internal BasicReport(Node node, byte[] payload) : base(node)
        {
            Value = payload[0];
        }

        public override string ToString()
        {
            return $"Value:{Value}";
        }
    }
}
