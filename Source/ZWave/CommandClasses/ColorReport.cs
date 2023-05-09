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
                Duration = PayloadConverter.ToTimeSpan(payload[3]);
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
