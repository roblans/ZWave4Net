using System;
using System.Linq;
using System.Text;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class ColorReport : NodeReport
    {
        public readonly ColorComponent Component;

        internal ColorReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 2)
                throw new ReponseFormatException($"Payload{BitConverter.ToString(payload)}");

            Component = new ColorComponent(payload[0], payload[1]);
        }

        public override string ToString()
        {
            return $"Component:{Component}";
        }
    }
}
