using System;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class SwitchBinaryReport : NodeReport
    {
        private const byte UNKNOWN = 0xFE;

        public readonly bool? Value;
        public readonly TimeSpan Duration;


        internal SwitchBinaryReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 1)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            if (payload[0] == UNKNOWN)
                Value = null;
            else
                Value = payload[0] != 0x0; //Values 0x1 - 0xFF = On

            //Version 2
            if (payload.Length > 1)
            {
                if (payload[1] == UNKNOWN || payload[1] == 0x0)
                    Duration = TimeSpan.Zero;
                if (payload[1] < 0x80)
                    Duration = new TimeSpan(0, 0, payload[1]);
                else
                    Duration = new TimeSpan(0, payload[1] - 0x80, 0);
            }
            else
                Duration = TimeSpan.Zero;
        }

        public override string ToString()
        {
            return $"Value:{Value},Duration:{Duration}";
        }
    }
}
