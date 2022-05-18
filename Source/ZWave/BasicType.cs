using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave
{
    public enum BasicType : byte
    {
        Unknown = 0x00,
        Controller = 0x01,
        StaticController = 0x02,
        Slave = 0x03,
        RoutingSlave = 0x04
    }
}
