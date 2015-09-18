using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net.Driver.Communication
{
    enum TransmitOptions : byte
    {
        Ack = 0x01,
        LowPower = 0x02,
        AutoRoute = 0x04,
        Explore = 0x20,
    }
}
