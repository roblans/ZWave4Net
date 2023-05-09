using System;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class ColorReport : NodeReport
    {
        public readonly ColorComponent CurrentValue;
        public readonly ColorComponent TargetValue;
        public readonly TimeSpan Duration;

        internal ColorReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 2)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            CurrentValue = new ColorComponent((ColorComponentType)payload[0], payload[1]);
           
            if (payload.Length >= 4)
            {
                //Version 3
                TargetValue = new ColorComponent((ColorComponentType)payload[0], payload[2]);
                if (payload[3] == 0xFE || payload[3] == 0x0)
                    Duration = TimeSpan.Zero;
                if (payload[3] < 0x80)
                    Duration = new TimeSpan(0, 0, payload[3]);
                else
                    Duration = new TimeSpan(0, payload[3] - 0x80, 0);
            }
            else
            {
                //Version 1 - 2
                TargetValue = CurrentValue;
                Duration = TimeSpan.Zero;
            }
        }

        public override string ToString()
        {
            return $"Target:{TargetValue}";
        }
    }
}
