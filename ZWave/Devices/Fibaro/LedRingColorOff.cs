using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Devices.Fibaro
{
    public enum LedRingColorOff : byte
    {
        NoChange = 0x00,
        White = 0x01,
        Red = 0x02,
        Green = 0x03,
        Blue = 0x04,
        Yellow = 0x05,
        Cyan = 0x06,
        Magenta = 0x07,
        Off = 0x08,
    }
}
