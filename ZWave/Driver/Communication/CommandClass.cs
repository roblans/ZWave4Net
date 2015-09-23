using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Driver.Communication
{
    public enum CommandClass : byte
    {
        Basic = 0x20,
        SwitchBinary = 0x25,
        ManufacturerSpecific = 0x72,
    }
}
