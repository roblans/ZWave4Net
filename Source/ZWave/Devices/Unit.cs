using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Devices
{
    public enum Unit
    {
        Celsius,
        Watt,
        Lux,
        KiloWattHour,
        Humidity,
        Ultraviolet,
        SeismicIntensity,
        Smoke,
        Percentage,
        Fahrenheit,
    }

    public static partial class Extentions
    {
        public static string GetSymbol(this Unit unit)
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
                    return "";
                case Unit.SeismicIntensity:
                    return "";
                case Unit.Smoke:
                    return "";
                case Unit.Percentage:
                    return "%";
                case Unit.Fahrenheit:
                    return "°F";
            }
            return string.Empty;
        }
    }
}
