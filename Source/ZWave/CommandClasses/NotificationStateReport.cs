using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class NotificationStateReport : NodeReport
    {
        public NotificationState[] SupportedStates { get; protected set; }

        internal NotificationStateReport(Node node, NotificationType notificationType, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 1)
                throw new ReponseFormatException($"The response was not in the expected format. Report: {GetType().Name}, Payload: {BitConverter.ToString(payload)}");

            byte len = (byte)(payload[0] & 0x1F);
            BitArray array = new BitArray(payload.Skip(1).ToArray());
            List<NotificationState> states = new List<NotificationState>();
            for (byte i = 0; i < len; i++)
            {
                if (array[i])
                    states.Add((NotificationState)(((int)notificationType << 8) | i));
            }
            SupportedStates = states.ToArray();
        }

        public override string ToString()
        {
            return $"Supported States:{string.Join(",", SupportedStates)}";
        }
    }
}
