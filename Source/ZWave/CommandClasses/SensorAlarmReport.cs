using System;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class SensorAlarmReport : NodeReport
    {
        public readonly byte Source;
        public readonly NotificationType Type;
        public readonly byte Level;
        public readonly ushort Duration;

        internal SensorAlarmReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 3)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            Source = payload[0];
            Type = (NotificationType)payload[1];
            Level = payload[2];
            Duration = (ushort)(payload[3] << 8 | payload[4]);
        }

        public override string ToString()
        {
            return $"Source:{Source}, Type:{Type}, Level:{Level}, Duration:{Duration}";
        }
    }
}
