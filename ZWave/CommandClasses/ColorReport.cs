using System;
using System.Linq;
using System.Text;

namespace ZWave.CommandClasses
{
    public class ColorReport : NodeReport
    {
        public readonly ColorComponent Component;

        internal ColorReport(Node node, byte[] payload) : base(node)
        {
            Component = new ColorComponent(payload[0], payload[1]);
        }

        public override string ToString()
        {
            return $"Component:{Component}";
        }
    }
}
