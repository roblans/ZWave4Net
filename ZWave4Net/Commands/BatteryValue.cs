using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net.Commands
{
    public class BatteryValue
    {
        public readonly Quantity<byte> Level;

        public BatteryValue(byte value) 
        {
            Level = new Quantity<byte>(value, "%");
        }

        public static BatteryValue Parse(byte[] payload)
        {
            var value = payload.First();
            if (value == 0xFF)
            {
                value = 0;
            }
            return new BatteryValue(value);
        }

    }
}
