using System;
using System.Collections.Generic;
using System.Text;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class MultiChannelReport : NodeReport
    {
        public readonly byte Byte0;
        public readonly byte Byte1;
        public readonly byte Byte2;
        public readonly byte Byte3;

        public readonly byte Value;

        internal MultiChannelReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 5)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            Byte0 = payload[0];
            Byte1 = payload[1];
            Byte2 = payload[2];
            Byte3 = payload[3];
            Value = payload[4];
        }

        public override string ToString()
        {
            return $"Byte0:{Byte0}. Byte1:{Byte1}. Byte2:{Byte2}. Byte3:{Byte3}. Value:{Value}";
        }
    }
}
