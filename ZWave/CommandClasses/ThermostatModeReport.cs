using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class ThermostatModeReport : NodeReport
    {
        public readonly ThermostatSetpointType Type;

        internal ThermostatModeReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 1)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            Type = (ThermostatSetpointType)(payload[0] & 0x1F);
        }
        
        public override string ToString()
        {
            return $"Type:{Type}";
        }
    }
}
