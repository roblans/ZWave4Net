using System;
using System.Collections.Generic;
using System.Text;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class VersionCommandClassReport : NodeReport
    {
        public readonly CommandClass Class;
        public readonly byte Version;

        internal VersionCommandClassReport(Node node, byte[] payload) : base(node)
        {
            Class = (CommandClass)Enum.ToObject(typeof(CommandClass), payload[0]);
            Version = payload[1];
        }

        public override string ToString()
        {
            return $"Class:{Class}, Version:{Version}";
        }
    }
}
