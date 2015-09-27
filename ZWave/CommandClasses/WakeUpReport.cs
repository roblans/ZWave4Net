using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class WakeUpReport : NodeReport
    {
        public readonly bool Awake;

        internal WakeUpReport(Node node) : base(node)
        {
            Awake = true;
        }

        public override string ToString()
        {
            return $"Awake:{Awake}";
        }
    }
}
