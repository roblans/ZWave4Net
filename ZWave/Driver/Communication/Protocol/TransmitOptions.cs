using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Driver.Communication.Protocol
{
    enum TransmitOptions : byte
    {
        Ack = 0x01,
        LowPower = 0x02,
        AutoRoute = 0x04,
        ForceRoute = 0x20,
    }
}
