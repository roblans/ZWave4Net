using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class AlarmSupportedReport : NodeReport
    {
        public readonly bool CustomV1Types;
        public readonly NotificationType[] SupportedAlarms;

        internal AlarmSupportedReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 1)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            CustomV1Types = ((payload[0] & 0x80) == 0x80);
            List<NotificationType> types = new List<NotificationType>();
            BitArray bitmask = new BitArray(payload.Skip(1).ToArray());
            for (int i = 0; i < bitmask.Length; i++)
            {
                if (bitmask[i])
                    types.Add((NotificationType)i);
            }
            SupportedAlarms = types.ToArray();
        }

        public override string ToString()
        {
            return $"Supported:{string.Join(",", SupportedAlarms)}";
        }
    }
}
