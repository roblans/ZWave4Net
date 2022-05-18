using System;
using System.Collections.Generic;
using System.Linq;

namespace ZWave.CommandClasses
{
    public class ScheduleData
    {
        [Flags]
        public enum Weekdays : byte
        {
            Monday = 1 << 0,
            Tuesday = 1 << 1,
            Wednesday = 1 << 2,
            Thursday = 1 << 3,
            Friday = 1 << 4,
            Saturday = 1 << 5,
            Sunday = 1 << 6,

            Workdays = Monday | Tuesday | Wednesday | Thursday | Friday,
            All = Monday | Tuesday | Wednesday | Thursday | Friday | Saturday | Sunday
        }

        public ScheduleState State { get; set; }
        public int? StartYear { get; set; }
        public int? StartMonth { get; set; }
        public int? StartDayOfMonth { get; set; }
        public Weekdays StartWeekdays { get; set; }
        public int? StartHour { get; set; }
        public int? StartMinute { get; set; }
        public TimeSpan? Duration { get; set; }
        public TimeSpan? RecurrenceInterval { get; set; } // Schedule v3
        public ScheduleOverrideType OverrideType { get; set; }
        public ScheduleCommand[] Commands { get; set; }

        private const byte DurationTypeMinutes = 0x00;
        private const byte DurationTypeHours = 0x01;
        private const byte DurationTypeDays = 0x02;
        private const byte DurationTypeOverride = 0x03;
        private const byte StartYearNotSpecified = 0xFF;
        private const byte StartMonthNotSpecified = 0x00;
        private const byte StartDayOfMonthNotSpecified = 0x00;
        private const byte StartHourNotSpecified = 0x1F;
        private const byte StartMinuteNotSpecified = 0x3F;
        private const int YearOffset = 2000;

        public static ScheduleData FromPayload(byte[] payload)
        {
            return new ScheduleData(payload);
        }

        public ScheduleData()
        {
        }

        private ScheduleData(byte[] payloadIncludingCommand)
        {
            StartYear = ParseYear(payloadIncludingCommand[2]);
            State = ParseState((byte)(payloadIncludingCommand[3] >> 4));
            StartMonth = ParseMonth((byte)(payloadIncludingCommand[3] & 0b00001111));
            StartDayOfMonth = ParseDayOfMonth((byte)(payloadIncludingCommand[4] & 0b00011111));
            StartWeekdays = ParseWeekdays((byte)(payloadIncludingCommand[5] & 0b01111111));
            StartHour = ParseHour((byte)(payloadIncludingCommand[6] & 0b00011111));
            StartMinute = ParseMinute((byte)(payloadIncludingCommand[7] & 0b00111111));
            Duration = ParseDuration((byte)(payloadIncludingCommand[6] >> 5),
                payloadIncludingCommand[8], payloadIncludingCommand[9]);
            OverrideType = ParseOverride((byte)(payloadIncludingCommand[6] >> 5), payloadIncludingCommand[9]);

            byte numCommands = payloadIncludingCommand[11];
            Commands = ParseCommands(numCommands, payloadIncludingCommand.Skip(12));
        }

        private static ScheduleCommand[] ParseCommands(byte numCommands, IEnumerable<byte> commandBytes)
        {
            var result = new ScheduleCommand[numCommands];
            int commandIndex = 0;

            var enumerator = commandBytes.GetEnumerator();
            while (true)
            {
                if (commandIndex >= numCommands || !enumerator.MoveNext())
                {
                    // no more commands
                    break;
                }

                byte length = enumerator.Current;
                var command = new byte[length];
                for (byte i = 0; i < length; ++i)
                {
                    if (!enumerator.MoveNext())
                    {
                        // Should not happen
                        break;
                    }

                    command[i] = enumerator.Current;
                }

                result[commandIndex] = new ScheduleCommand { Command = command };
                commandIndex++;
            }

            return result;
        }

        private int? ParseYear(byte yearByte)
        {
            return yearByte == StartYearNotSpecified ? new int?() : (yearByte + YearOffset);
        }

        private ScheduleState ParseState(byte stateByte)
        {
            return (ScheduleState)stateByte;
        }

        private int? ParseMonth(byte monthByte)
        {
            return monthByte == StartMonthNotSpecified ? new int?() : monthByte;
        }

        private int? ParseDayOfMonth(byte dayOfMonthByte)
        {
            return dayOfMonthByte == StartDayOfMonthNotSpecified ? new int?() : dayOfMonthByte;
        }

        private Weekdays ParseWeekdays(byte weekdaysByte)
        {
            return (Weekdays)weekdaysByte;
        }

        private int? ParseHour(byte hourByte)
        {
            return hourByte == StartHourNotSpecified ? new int?() : hourByte;
        }

        private int? ParseMinute(byte minuteByte)
        {
            return minuteByte == StartMinuteNotSpecified ? new int?() : minuteByte;
        }

        private TimeSpan? ParseDuration(byte durationType, byte durationByte1, byte durationByte2)
        {
            int durationValue = (durationByte1 << 8) + durationByte2;

            if (durationType == DurationTypeMinutes)
            {
                return TimeSpan.FromMinutes(durationValue);
            }
            if (durationType == DurationTypeHours)
            {
                return TimeSpan.FromHours(durationValue);
            }
            if (durationType == DurationTypeDays)
            {
                return TimeSpan.FromDays(durationValue);
            }

            return null;
        }

        private ScheduleOverrideType ParseOverride(byte durationType, byte durationByte2)
        {
            if (durationType == DurationTypeOverride)
            {
                return (ScheduleOverrideType)durationByte2;
            }

            return ScheduleOverrideType.NoOverride;
        }

        public byte[] ToPayload(byte scheduleId, byte? scheduleIdBlock = null)
        {
            int numBytes = 12 + Commands.Sum(cmd => cmd.Command.Length + 1);

            var bytes = new byte[numBytes];

            byte durationType, durationByte1, durationByte2;
            if (Duration.HasValue)
            {
                var duration = Duration.Value;
                ushort durationNumber;
                if (Duration >= TimeSpan.FromHours(60000))
                {
                    durationType = DurationTypeDays;
                    durationNumber = (ushort)Math.Min(duration.Days, ushort.MaxValue);
                }
                else if (Duration >= TimeSpan.FromMinutes(60000))
                {
                    durationType = DurationTypeHours;
                    durationNumber = (ushort)(duration.Days * 24 + duration.Hours);
                }
                else
                {
                    durationType = DurationTypeMinutes;
                    durationNumber = (ushort)((duration.Days * 24 + duration.Hours) * 60 + duration.Minutes);
                }
                durationByte1 = (byte)(durationNumber / 256);
                durationByte2 = (byte)(durationNumber % 256);
            }
            else
            {
                durationType = DurationTypeOverride;
                durationByte1 = 0;
                durationByte2 = (byte)OverrideType;
            }

            bytes[0] = scheduleId;
            bytes[1] = scheduleIdBlock.GetValueOrDefault(1);
            bytes[2] = (byte)(StartYear.HasValue ? StartYear - YearOffset : StartYearNotSpecified);
            bytes[3] = (byte)((int)State << 4 | StartMonth.GetValueOrDefault(StartMonthNotSpecified));
            bytes[4] = (byte)StartDayOfMonth.GetValueOrDefault(StartDayOfMonthNotSpecified);
            bytes[5] = (byte)(durationType << 5 | (int)StartWeekdays);
            bytes[6] = (byte)StartHour.GetValueOrDefault(StartHourNotSpecified);
            bytes[7] = (byte)StartMinute.GetValueOrDefault(StartMinuteNotSpecified);
            bytes[8] = durationByte1;
            bytes[9] = durationByte2;
            bytes[10] = 0; // reports to follow
            bytes[11] = (byte)Commands.Length;

            int index = 12;
            foreach (var cmd in Commands)
            {
                bytes[index] = (byte)cmd.Command.Length;
                index++;
                Array.Copy(cmd.Command, 0, bytes, index, cmd.Command.Length);
                index += cmd.Command.Length;
            }

            return bytes;
        }
    }
}
