using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZWave.Channel;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class MeterReport : NodeReport
    {
        public readonly MeterType Type;
        public readonly float Value;
        public readonly string Unit;
        public readonly Enum Scale;

        internal MeterReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 3)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            var scale = default(byte);
            Type = (MeterType)(payload[0] & 0x1F);
            Value = PayloadConverter.ToFloat(payload.Skip(1).ToArray(), out scale);
            Unit = GetUnit(Type, scale);
            Scale = GetScale(Type, scale);
        }

        public static string GetUnit(MeterType type, byte scale)
        {
            var electricityUnits = new[] { "kWh", "kVAh", "W", "pulses", "V", "A", "Power Factor", "" };
            var gasUnits = new[] { "cubic meters", "cubic feet", "", "pulses", "", "", "", "" };
            var waterUnits = new[] { "cubic meters", "cubic feet", "US gallons",  "pulses", "", "", "", ""};

            switch (type)
            {
                case MeterType.Electric: return electricityUnits[scale];
                case MeterType.Gas: return gasUnits[scale];
                case MeterType.Water: return waterUnits[scale];
                default: return string.Empty;
            }
        }

        public static Enum GetScale(MeterType type, byte scale)
        {
            switch (type)
            {
                case MeterType.Electric:
                    return (ElectricMeterScale)scale;
                case MeterType.Gas:
                    return (GasMeterScale)scale;
                case MeterType.Water:
                    return (WaterMeterScale)scale;
            }
            return default(Enum);
        }

        public override string ToString()
        {
            return $"Type:{Type}, Value:\"{Value} {Unit}\"";
        }
    }
}
