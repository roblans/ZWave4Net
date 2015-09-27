using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.CommandClasses
{
    public class VersionReport : NodeReport
    {
        public readonly string Library;
        public readonly string Application;
        public readonly string Protocol;

        internal VersionReport(Node node, byte[] payload) : base(node)
        {
            Library = payload[0].ToString("d");
            Protocol = payload[1].ToString("d") + "." + payload[2].ToString("d2");
            Application = payload[3].ToString("d") + "." + payload[4].ToString("d2");
        }

        public override string ToString()
        {
            return $"Library:{Library}, Protocol:{Protocol}, Application:{Application}";
        }
    }
}
