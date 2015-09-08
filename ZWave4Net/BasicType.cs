using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net
{
    public enum BasicType
    {
        Unknown = 0x00,
        Controller = 0x01,
        StaticController = 0x02,
        Slave = 0x03,
        RoutingSlave = 0x04
    }
}
