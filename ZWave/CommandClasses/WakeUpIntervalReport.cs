using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using ZWave.Channel;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class WakeUpIntervalReport : NodeReport
    {
        public readonly TimeSpan Interval = TimeSpan.Zero;
        public readonly byte TargetNodeID;

        internal WakeUpIntervalReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));

            if (payload.Length >= 4)
            {
                // 3 bytes for interval
                var interval = PayloadConverter.ToUInt32(new byte[] { 0 }.Concat(payload).ToArray());
                Interval = TimeSpan.FromSeconds(interval);

                // one byte for targetnode
                TargetNodeID = payload[3];
                return;
            }
            
            // some interval reports received are validly formatted(proper checksum, etc.) but only have length of 1 byte (0x00). Not sure what this means
            if (payload.Length == 1 && payload[0] == 0x00)
            {
                Interval = TimeSpan.Zero;
                TargetNodeID = 0;
                return;
            }
            throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");
        }

        public override string ToString()
        {
            return $"Interval:{Interval}, TargetNode:{TargetNodeID:D3}";
        }
    }
}
