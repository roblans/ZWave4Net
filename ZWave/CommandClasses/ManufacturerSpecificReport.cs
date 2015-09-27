using System;
using System.Collections.Generic;
using System.Linq;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class ManufacturerSpecificReport : NodeReport
    {
        public readonly ushort ManufacturerID;
        public readonly ushort ProductType;
        public readonly ushort ProductID;

        internal ManufacturerSpecificReport(Node node, byte[] payload) : base(node)
        {
            ManufacturerID = PayloadConverter.ToUInt16(payload, 0);
            ProductType = PayloadConverter.ToUInt16(payload, 2);
            ProductID = PayloadConverter.ToUInt16(payload, 4);
        }

        public override string ToString()
        {
            return $"ManufacturerID:{ManufacturerID:X4}, ProductType:{ProductType:X4}, ProductID:{ProductID:X4}";
        }
    }
}
