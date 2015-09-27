using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.CommandClasses
{
    public enum MeterType : byte
    {
        Unknown = 0x00,
        Electric = 0x01,
        Gas = 0x02,
        Water = 0x03,
    }
}
