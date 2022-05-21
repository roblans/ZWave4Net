using System;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class ThermostatFanModeReport : NodeReport
    {
        public readonly ThermostatFanModeValue Mode;

        internal ThermostatFanModeReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 1)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            Mode = (ThermostatFanModeValue)payload[0];
        }

        public override string ToString()
        {
            return $"Mode:{Mode}";
        }
    }
}
