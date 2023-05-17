using System;
using System.Collections;
using System.Linq;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class SensorMultilevelSupportedScaleReport : NodeReport
    {
        public SensorType SensorType;
        public BitArray SupportedScales;

        public SensorMultilevelSupportedScaleReport(Node node, byte[] payload)
            : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 2)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");
            SensorType = (SensorType)payload[0];
            SupportedScales = new BitArray(payload.Skip(1).ToArray());
        }
    }
}
