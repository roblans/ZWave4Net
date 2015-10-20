using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Channel;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class SwitchBinaryReport : NodeReport
    {
        public readonly bool Value;

        internal SwitchBinaryReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 1)
                throw new ReponseFormatException($"Payload{BitConverter.ToString(payload)}");

            Value = payload[0] == 0xFF;
        }

        public override string ToString()
        {
            return $"Value:{Value}";
        }
    }
}
