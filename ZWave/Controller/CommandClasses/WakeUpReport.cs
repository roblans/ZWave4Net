using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using ZWave.Channel;

namespace ZWave.Controller.CommandClasses
{
    public class WakeUpReport : NodeReport
    {
        public readonly TimeSpan Interval;
        public readonly byte TargetNodeID;

        internal WakeUpReport(Node node, byte[] payload) : base(node)
        {
            // 3 bytes for interval
            var interval = PayloadConverter.ToUInt32(new byte[] { 0 }.Concat(payload).ToArray());
            Interval = TimeSpan.FromSeconds(interval);
            TargetNodeID = payload[3];
        }

        public override string ToString()
        {
            return $"Interval:{Interval}, TargetNode:{TargetNodeID:D3}";
        }
    }
}
