using System;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class ScheduleReport : NodeReport
    {
        public readonly byte ID;

        public readonly ScheduleData Data;

        internal ScheduleReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 12)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            ID = payload[0];
            Data = ScheduleData.FromPayload(payload);
        }

        public override string ToString()
        {
            return $"ID:{ID}";
        }
    }
}
