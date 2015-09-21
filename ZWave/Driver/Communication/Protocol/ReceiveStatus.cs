using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Driver.Communication.Protocol
{
    enum ReceiveStatus : byte
    {
        Single = 0x00,
        RoutedBusy = 0x01,
        LowPower = 0x02,
        TypeBroad = 0x04,
        TypeMulti = 0x08,
        TypeMask = 0x0C,
        ForeignFrame = 0x40,
    };
}
