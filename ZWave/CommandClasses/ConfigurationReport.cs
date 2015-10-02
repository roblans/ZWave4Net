using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.CommandClasses
{
    public class ConfigurationReport : NodeReport
    {
        public readonly byte Parameter;
        public readonly long Value;

        internal ConfigurationReport(Node node, byte[] payload) : base(node)
        {
            Parameter = payload[0];
            switch (payload[1])
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
            }
        }

        public override string ToString()
        {
            return $"Parameter:{Parameter}, Value:{Value}";
        }
    }
}
