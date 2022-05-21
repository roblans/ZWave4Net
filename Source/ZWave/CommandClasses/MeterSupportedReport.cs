using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZWave.Channel;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class MeterSupportedReport : NodeReport
    {
        public readonly bool CanReset;
        public readonly MeterType Type;
        public readonly Enum[] Scales;
        public readonly string[] Units;

        internal MeterSupportedReport(Node node, byte[] payload) : base(node)
        {
            if (payload.Length < 2)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            CanReset = (payload[0] & 0x80) != 0;
            Type = (MeterType)Enum.ToObject(typeof(MeterType), payload[0] & 0x1F);

            var units = new List<string>();
            var scales = new List<Enum>();
            for (byte i = 0; i < 8; ++i)
            {
                if ((payload[1] & (1 << i)) == (1 << i))
                {
                    units.Add(MeterReport.GetUnit(Type, i));
                    scales.Add(MeterReport.GetScale(Type, i));
                }
            }
            Units = units.ToArray();
            Scales = scales.ToArray();
        }

        public override string ToString()
        {
            return $"CanReset:{CanReset}, Type:{Type}, Scales:[{string.Join(", ", Scales.Cast<object>())}]";
        }
    }
}
