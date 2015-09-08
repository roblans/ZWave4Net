using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net.Commands
{
    public class ProductData
    {
        public readonly ushort ManufacturerID;
        public readonly ushort ProductType;
        public readonly ushort ProductID;

        private ProductData(ushort manufacturerID, ushort productType, ushort productID)
        {
            ManufacturerID = manufacturerID;
            ProductType = productType;
            ProductID = productID;
        }

        public override string ToString()
        {
            return string.Format("ManufacturerID = {0:X4}, ProductType = {1:X4}, ProductID = {2:X4}", ManufacturerID, ProductType, ProductID);
        }

        public static ProductData Parse(byte[] data)
        {
            data = data.Reverse().ToArray();
            var productID = BitConverter.ToUInt16(data, 0);
            var productType = BitConverter.ToUInt16(data, 2);
            var manufacturerID = BitConverter.ToUInt16(data, 4);

            return new ProductData(manufacturerID, productType, productID);
        }
    }
}
