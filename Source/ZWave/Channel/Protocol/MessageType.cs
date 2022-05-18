using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Channel.Protocol
{
    enum MessageType : byte
    {
        Request = 0x00,
        Response = 0x01,
        GetVersion = 0x15,
        MemoryGetId = 0x20,
        ClockSet = 0x30
    }
}
