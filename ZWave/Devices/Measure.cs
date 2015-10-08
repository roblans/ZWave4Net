using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Devices
{
    public class Measure
    {
        public readonly float Value;
        public readonly Unit Unit;
        public readonly string Symbol;

        public Measure(float value, Unit unit)
        {
            Value = value;
            Unit = unit;
            Symbol = GetSymbol(unit);
        }

        private static string GetSymbol(Unit unit)
        {
            switch (unit)
            {
                case Unit.Celsius:
                    return "°C";
                case Unit.Watt:
                    return "W";
                case Unit.Lux:
                    return "lux";
                case Unit.KiloWattHour:
                    return "kWh";
                case Unit.Humidity:
                    return "%";
                case Unit.Ultraviolet:
                    return "?"; // ToDo
                case Unit.SeismicIntensity:
                    return "?"; // ToDo
                case Unit.Smoke:
                    return "?"; // ToDo
            }
            return null;
        }

        public override string ToString()
        {
            return $"{Value} {Symbol}";
        }
    }

    public enum Unit
    {
        Celsius,
        Watt,
        Lux,
        KiloWattHour,
        Humidity,
        Ultraviolet,
        SeismicIntensity,
        Smoke
    }
}
