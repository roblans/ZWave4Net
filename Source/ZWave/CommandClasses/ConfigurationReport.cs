using System;
using System.Collections.Generic;
using System.Text;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class ConfigurationReport : NodeReport
    {
        public readonly byte Parameter;
        public readonly byte Size;
        public readonly object Value;

        internal ConfigurationReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 3)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            Parameter = payload[0];
            Size = payload[1];

            try
            {
                switch (Size)
                {
                    case 1:
                        Value = PayloadConverter.ToInt8(payload, 2);
                        break;
                    case 2:
                        Value = PayloadConverter.ToInt16(payload, 2);
                        break;
                    case 4:
                        Value = PayloadConverter.ToInt32(payload, 2);
                        break;
                    case 8:
                        Value = PayloadConverter.ToInt64(payload, 2);
                        break;
                    default:
                        throw new NotSupportedException($"Size:{Size} is not supported");
                }
            }
            catch (Exception ex)
            {
                throw new ReponseFormatException($"The response was not in the expected format. Payload{BitConverter.ToString(payload)}", ex);
            }
        }

        public override string ToString()
        {
            return $"Parameter:{Parameter}, Value:{Value}";
        }
    }
}
