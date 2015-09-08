using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net
{
    [Flags()]
    public enum Security
    {
        Nonde = 0x00,
        Security = 0x01,
        Controller = 0x02,
        SpecificDevice = 0x04,
        RoutingSlave = 0x08,
        BeamCapability = 0x10,
        Sensor250ms = 0x20,
        Sensor1000ms = 0x40,
        OptionalFunctionality = 0x80
    }
}
