using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Driver.Communication
{
    public enum CommandClass : byte
    {
        Basic = 0x20,
        SwitchBinary = 0x25,
        SensorBinary = 0x30,
        SensorMultiLevel = 0x31,
        Alarm = 0x71,
        ManufacturerSpecific = 0x72,
        Battery = 0x80,
        Association = 0x85,
        SensorAlarm = 0x9C,
    }
}
