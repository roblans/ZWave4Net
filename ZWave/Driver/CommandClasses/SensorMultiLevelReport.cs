using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZWave.Driver.Communication;

namespace ZWave.Driver.CommandClasses
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
            Value = ParseValue(payload.Skip(1).ToArray(), out Scale);
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

        private float ParseValue(byte[] payload, out byte scale)
        {
            // http://www.google.nl/url?q=http://www.cooperindustries.com/content/dam/public/wiringdevices/products/documents/technical_specifications/aspirerf_adtechguide_100713.pdf&sa=U&ved=0CBwQFjAAOApqFQoTCN-G7K7djcgCFeZr2wod5eILEA&usg=AFQjCNGzaMFiMosWoKLG-Wvo1A2p5QDbTw
            // http://www.google.nl/url?q=http://svn.linuxmce.org/trac/browser/trunk/src/ZWave/ZWApi.cpp%3Frev%3D27702&sa=U&ved=0CCYQFjAEOBRqFQoTCKboxvHkjcgCFYkH2wodxRoB0w&usg=AFQjCNFg3uMoEuAIX6R61kDS-5Q9C-F-GA
            // https://searchcode.com/codesearch/view/90040075/
            // bits 7,6,5: precision, bits 4,3: scale, bits 2,1,0 : size
            var precision = (byte)((payload[0] & 0xE0) >> 5);
            scale = (byte)((payload[0] & 0x18) >> 3);
            var size = (byte)(payload[0] & 0x07);

            var value = (ulong)0;
            for(int i = 0; i < size; i++)
            {
                value <<= sizeof(byte);
                value |= payload[i + 1];
            }

            // deal with sign extension. All values are signed
            if ((payload[1] & 0x80) == 0x80)
            {
                value |= (0xFFFFFFFFFFFFFFFF << size);
            }

            return (float)(value / Math.Pow(10, precision));
        }

        public override string ToString()
        {
            return $"Type:{Type}, Value:\"{Value} {Unit}\"";
        }
    }
}
