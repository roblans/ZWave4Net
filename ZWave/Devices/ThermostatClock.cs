using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Devices
{
    public class ThermostatClock
    {
        public readonly DayOfWeek DayOfWeek;
        public readonly byte Hour;
        public readonly byte Minute;

        public ThermostatClock(DayOfWeek dayOfWeek, byte hour, byte minute)
        {
            DayOfWeek = dayOfWeek;
            Hour = hour;
            Minute = minute;
        }

        public ThermostatClock(DateTime date)
            : this(date.DayOfWeek, (byte)date.Hour, (byte)date.Minute)
        {
        }


        public override string ToString()
        {
            return $"{DayOfWeek} {Hour:D2}:{Minute:D2}";
        }

    }
}
