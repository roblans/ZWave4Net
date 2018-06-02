using System;
using System.Collections.Generic;
using System.Text;
using ZWave.Channel;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class MultiChannelCapabilityReport : NodeReport
    {
        private const int _numberOfFixedParams = 3;

        public bool IsDynamic { get; }

        public byte EndPoint { get; }

        public CommandClass[] SupportedCommandClasses { get; }

        public MultiChannelCapabilityReport(Node node, byte[] payload)
            : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < _numberOfFixedParams)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            IsDynamic = (payload[0] & 0x80) > 0;
            EndPoint = (byte)(payload[0] & 0x7F);

            // Ignoring payload[1] - Generic Device Class
            // Ignoring payload[2] - Specific Device Class
            //
            SupportedCommandClasses = new CommandClass[payload.Length - _numberOfFixedParams];
            for (int i = 0; i < SupportedCommandClasses.Length; i++)
            {
                SupportedCommandClasses[i] = (CommandClass)payload[_numberOfFixedParams + i];
            }
        }
    }
}
