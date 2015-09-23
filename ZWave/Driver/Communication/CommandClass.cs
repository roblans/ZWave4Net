using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Driver.Communication
{
    public enum CommandClass : byte
    {
        Basic = 0x20,
        SwitchBinary = 0x25,
        Alarm = 0x71,
        ManufacturerSpecific = 0x72,
        Battery = 0x80,
        Association = 0x85,
    }
}
