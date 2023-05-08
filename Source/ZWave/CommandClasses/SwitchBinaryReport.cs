using System;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class SwitchBinaryReport : NodeReport
    {
        private const byte UNKNOWN = 0xFE;

        public readonly bool? CurrentValue;
        public readonly bool? TargetValue;
        public readonly TimeSpan Duration;


        internal SwitchBinaryReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 1)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            if (payload[0] == UNKNOWN)
                CurrentValue = null;
            else
                CurrentValue = payload[0] != 0x0; //Values 0x1 - 0xFF = On

            //Version 2
            if (payload.Length > 2)
            {
                if (payload[1] == UNKNOWN)
                    TargetValue = null;
                else
                    TargetValue = payload[1] != 0x0; //Values 0x1 - 0xFF = On
                if (payload[2] == UNKNOWN || payload[2] == 0x0)
                    Duration = TimeSpan.Zero;
                if (payload[2] < 0x80)
                    Duration = new TimeSpan(0, 0, payload[2]);
                else
                    Duration = new TimeSpan(0, payload[2] - 0x80, 0);
            }
            else
            {
                Duration = TimeSpan.Zero;
                TargetValue = CurrentValue;
            }
        }

        public override string ToString()
        {
            return $"Value:{Value},Duration:{Duration}";
        }
    }
}
