using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class ThermostatSetpointReport : NodeReport
    {
        public readonly ThermostatSetpointType Type;
        public readonly float Value;
        public readonly string Unit;
        public readonly byte Scale;

        internal ThermostatSetpointReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 3)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            Type = (ThermostatSetpointType)(payload[0] & 0x1F);
            Value = PayloadConverter.ToFloat(payload.Skip(1).ToArray(), out Scale);
            Unit = GetUnit(Type, Scale);
        }

        public static string GetUnit(ThermostatSetpointType type, byte scale)
        {
            return (scale == 1 ? "°F" : "°C");
        }

        public override string ToString()
        {
            return $"Type:{Type}, Value:\"{Value} {Unit}\"";
        }
    }
}
