using System;
using System.Collections.Generic;
using System.Text;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class ClockReport : NodeReport
    {
        public readonly DayOfWeek DayOfWeek;
        public readonly byte Hour;
        public readonly byte Minute;

        internal ClockReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 2)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            var day = (byte)(payload[0] >> 5);
            switch(day)
            {
                case 1:
                    DayOfWeek = DayOfWeek.Monday;
                    break;
                case 2:
                    DayOfWeek = DayOfWeek.Tuesday;
                    break;
                case 3:
                    DayOfWeek = DayOfWeek.Wednesday;
                    break;
                case 4:
                    DayOfWeek = DayOfWeek.Thursday;
                    break;
                case 5:
                    DayOfWeek = DayOfWeek.Friday;
                    break;
                case 6:
                    DayOfWeek = DayOfWeek.Saturday;
                    break;
                case 7:
                    DayOfWeek = DayOfWeek.Sunday;
                    break;
            }
            Hour = (byte)(payload[0] & 0x1F);
            Minute = payload[1];
        }

        public override string ToString()
        {
            return $"{DayOfWeek} {Hour:D2}:{Minute:D2}";
        }
    }
}
