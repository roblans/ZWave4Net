using System;
using System.Linq;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class WakeUpCapabilitiesReport : NodeReport
    {
        public readonly TimeSpan Minimum = TimeSpan.Zero;
        public readonly TimeSpan Maximum = TimeSpan.Zero;
        public readonly TimeSpan Default = TimeSpan.Zero;
        public readonly TimeSpan Step = TimeSpan.Zero;
        public readonly bool WakeOnDemand = false;

        internal WakeUpCapabilitiesReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));

            if (payload.Length >= 12)
            {
                var interval = PayloadConverter.ToUInt32(payload.Take(3).Prepend((byte)0).ToArray());
                Minimum = TimeSpan.FromSeconds(interval);

                interval = PayloadConverter.ToUInt32(payload.Skip(3).Take(3).Prepend((byte)0).ToArray());
                Maximum = TimeSpan.FromSeconds(interval);

                interval = PayloadConverter.ToUInt32(payload.Skip(6).Take(3).Prepend((byte)0).ToArray());
                Default = TimeSpan.FromSeconds(interval);

                interval = PayloadConverter.ToUInt32(payload.Skip(9).Take(3).Prepend((byte)0).ToArray());
                Step = TimeSpan.FromSeconds(interval);

                if (payload.Length >= 13)
                    WakeOnDemand = (payload[12] & 0x1) == 0x1;
                return;
            }
            throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");
        }

        public override string ToString()
        {
            return $"Minimum:{Minimum}, Maximum:{Maximum}, Default:{Default}";
        }
    }
}
