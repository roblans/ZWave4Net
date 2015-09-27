using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.CommandClasses
{
    public class ConfigurationReport : NodeReport
    {
        public readonly byte Parameter;
        public readonly object Value;

        internal ConfigurationReport(Node node, byte[] payload) : base(node)
        {
            Parameter = payload[0];
            switch (payload[1])
            {
                case sizeof(sbyte):
                    Value = (sbyte)payload[2];
                    break;
                case sizeof(short):
                    Value = PayloadConverter.ToInt16(payload, 2);
                    break;
                case sizeof(int):
                    Value = PayloadConverter.ToInt32(payload, 2);
                    break;
                case sizeof(long):
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
