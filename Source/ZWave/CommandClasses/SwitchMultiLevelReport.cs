using System;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class SwitchMultiLevelReport : NodeReport
    {
        public readonly byte CurrentValue;
        public readonly byte TargetValue;
        public readonly TimeSpan Duration;

        internal SwitchMultiLevelReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));

            if (payload.Length == 1)
            {
                CurrentValue = TargetValue = payload[0];
                Duration = TimeSpan.Zero;
            }
            else if (payload.Length == 3)
            {
                CurrentValue =  payload[0];
                TargetValue = payload[1];
                Duration = PayloadConverter.ToTimeSpan(payload[2]);
            }
            else
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");
        }

        public override string ToString()
        {
            return $"CurrentValue:{CurrentValue}, TargetValue:{TargetValue}, Duration:{Duration}";
        }
    }
}
