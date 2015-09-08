using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net.Commands
{
    public class BatteryLevel : Quantity<byte>
    {
        public BatteryLevel(byte value) 
            : base(value, "%")
        {
        }
    }
}
