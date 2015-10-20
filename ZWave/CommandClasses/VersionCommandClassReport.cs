using System;
using System.Collections.Generic;
using System.Text;
using ZWave.Channel;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class VersionCommandClassReport : NodeReport
    {
        public readonly CommandClass Class;
        public readonly byte Version;

        internal VersionCommandClassReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 2)
                throw new ReponseFormatException($"The response was not in the expected format. Payload{BitConverter.ToString(payload)}");

            Class = (CommandClass)Enum.ToObject(typeof(CommandClass), payload[0]);
            Version = payload[1];
        }

        public override string ToString()
        {
            return $"Class:{Class}, Version:{Version}";
        }
    }
}
