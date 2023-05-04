using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.CommandClasses
{
    public enum BinarySensorType : byte
    {
        Reserved = 0x0,
        GeneralPurpose = 0x1,
        Smoke = 0x2,
        CarbonMonoxide = 0x3,
        CarbonDioxide = 0x4,
        Heat = 0x5,
        Water = 0x6,
        Freeze = 0x7,
        Tamper = 0x8,
        Aux = 0x9,
        DoorWindow = 0xA,
        Tilt = 0xB,
        Motion = 0xC,
        GlassBreak = 0xD,
        FirstSupported = 0xFF
    }
}
