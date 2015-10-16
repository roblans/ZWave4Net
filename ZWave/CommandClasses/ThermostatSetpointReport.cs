using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

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
            Type = (ThermostatSetpointType)(payload[0] & 0x1F);
            Value = PayloadConverter.ToSensorValue(payload.Skip(1).ToArray(), out Scale);
            Unit = GetUnit(Type, Scale);
        }

        public static string GetUnit(ThermostatSetpointType type, byte scale)
        {
            return "°C";
        }

        public override string ToString()
        {
            return $"Type:{Type}, Value:\"{Value} {Unit}\"";
        }
    }
}
