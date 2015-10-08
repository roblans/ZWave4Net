using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.CommandClasses
{
    public class ColorReport : NodeReport
    {
        public readonly byte[] Payload;

        internal ColorReport(Node node, byte[] payload) : base(node)
        {
            Payload = payload;
        }

        public override string ToString()
        {
            return $"Payload:{BitConverter.ToString(Payload)}% ";
        }
    }
}
