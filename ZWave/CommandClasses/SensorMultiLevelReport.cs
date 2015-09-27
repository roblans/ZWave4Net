using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class SensorMultiLevelReport : NodeReport
    {
        public readonly SensorType Type;
        public readonly float Value;
        public readonly string Unit;
        public readonly byte Scale;

        internal SensorMultiLevelReport(Node node, byte[] payload) : base(node)
        {
            Type = (SensorType)payload[0];
            Value = PayloadConverter.ToSensorValue(payload.Skip(1).ToArray(), out Scale);
            Unit = GetUnit(Type, Scale);
        }

        private static string GetUnit(SensorType type, byte scale)
        {
            var tankCapacityUnits = new[] { "l", "cbm", "gal" };
            var distanceUnits = new [] { "m", "cm", "ft" };

            switch (type)
            {
                case SensorType.Temperature: return(scale == 1 ? "F" : "C");
                case SensorType.General: return (scale == 1 ? "" : "%");
                case SensorType.Luminance: return(scale == 1 ? "lux" : "%");
                case SensorType.Power: return(scale == 1 ? "BTU/h" : "W");
                case SensorType.RelativeHumidity: return("%");
                case SensorType.Velocity: return(scale == 1 ? "mph" : "m/s");
                case SensorType.Direction: return("");
                case SensorType.AtmosphericPressure: return(scale == 1 ? "inHg" : "kPa");
                case SensorType.BarometricPressure: return(scale == 1 ? "inHg" : "kPa");
                case SensorType.SolarRadiation: return("W/m2");
                case SensorType.DewPoint: return(scale == 1 ? "in/h" : "mm/h");
                case SensorType.RainRate: return(scale == 1 ? "F" : "C");
                case SensorType.TideLevel: return(scale == 1 ? "ft" : "m");
                case SensorType.Weight: return(scale == 1 ? "lb" : "kg");
                case SensorType.Voltage: return(scale == 1 ? "mV" : "V");
                case SensorType.Current: return(scale == 1 ? "mA" : "A");
                case SensorType.CO2: return("ppm");
                case SensorType.AirFlow: return(scale == 1 ? "cfm" : "m3/h");
                case SensorType.TankCapacity: return(tankCapacityUnits[scale]);
                case SensorType.Distance: return(distanceUnits[scale]);
                default: return string.Empty;
            }
        }

        public override string ToString()
        {
            return $"Type:{Type}, Value:\"{Value} {Unit}\"";
        }
    }
}
