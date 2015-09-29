using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Devices.Fibaro
{
    public enum LedRingColorOn : byte
    {
        PowerLoadStep = 0x00,
        PowerLoadContinously = 0x01,
        White = 0x02,
        Red = 0x03,
        Green = 0x04,
        Blue = 0x05,
        Yellow = 0x06,
        Cyan = 0x07,
        Magenta = 0x08,
        Off = 0x09,
    }
}
