using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class Clock : CommandClassBase
    {
        enum command : byte
        {
            Set = 0x04,
            Get = 0x05,
            Report = 006,
        }

        public Clock(Node node) : base(node, CommandClass.Clock)
        {
        }

        public async Task<ClockReport> Get()
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get), command.Report);
            return new ClockReport(Node, response);
        }

        public async Task Set(DayOfWeek dayOfWeek, byte hour, byte minute)
        {
            var day = default(byte);
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    day = 1;
                    break;
                case DayOfWeek.Tuesday:
                    day = 2;
                    break;
                case DayOfWeek.Wednesday:
                    day = 3;
                    break;
                case DayOfWeek.Thursday:
                    day = 4;
                    break;
                case DayOfWeek.Friday:
                    day = 5;
                    break;
                case DayOfWeek.Saturday:
                    day = 6;
                    break;
                case DayOfWeek.Sunday:
                    day = 7;
                    break;
            }

            var payload = new byte[] { 0x00, 0x00 };
            payload[0] |= (byte)(day << 5);
            payload[0] |= (byte)(hour & 0x1F);
            payload[1] = minute;

            await Channel.Send(Node, new Command(Class, command.Set, payload));
        }
    }
}
