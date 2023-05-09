using System;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class BasicReport : NodeReport
    {
        public readonly byte CurrentValue;
        public readonly byte TargetValue;
        public readonly TimeSpan Duration;

        internal BasicReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 1)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            CurrentValue = payload[0];

            if (payload.Length >= 3)
            {
                //Version 2
                TargetValue = payload[1];
                Duration = PayloadConverter.ToTimeSpan(payload[2]);
            }
            else
            {
                //Version 1
                TargetValue = CurrentValue;
                Duration = TimeSpan.Zero;
            }
        }

        public override string ToString()
        {
            return $"Value:{CurrentValue}";
        }
    }
}
