using System;
using System.Collections.Generic;
using System.Linq;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class ThermostatFanModeSupportedValuesReport : NodeReport
    {
        public readonly ThermostatFanModeValue[] SupportedModes;

        internal ThermostatFanModeSupportedValuesReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 1)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            var set = new HashSet<ThermostatFanModeValue>();
            foreach (ThermostatFanModeValue value in Enum.GetValues(typeof(ThermostatFanModeValue)))
            {
                int bitIndex = (int)value;
                int byteIndex = bitIndex / 8;

                if (byteIndex >= payload.Length)
                {
                    break;
                }

                byte currentByte = payload[byteIndex];

                uint bit = 1U << (bitIndex % 8);
                if ((bit & currentByte) == bit)
                {
                    set.Add(value);
                }
            }

            SupportedModes = set.ToArray();
        }

        public override string ToString()
        {
            return $"SupportedModes:{SupportedModes}";
        }
    }
}
