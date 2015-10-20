using System;
using System.Collections.Generic;
using System.Text;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class BasicReport : NodeReport
    {
        public readonly byte Value;

        internal BasicReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 1)
                throw new ReponseFormatException($"The response was not in the expected format. Payload{BitConverter.ToString(payload)}");

            Value = payload[0];
        }

        public override string ToString()
        {
            return $"Value:{Value}";
        }
    }
}
