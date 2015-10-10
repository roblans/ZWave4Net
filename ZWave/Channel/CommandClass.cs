using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Channel
{
    public enum CommandClass : byte
    {
        Basic = 0x20,
        SwitchBinary = 0x25,
        SensorBinary = 0x30,
        SensorMultiLevel = 0x31,
        Meter = 0x32,
        Color = 0x33,
        Configuration = 0x70,
        Alarm = 0x71,
        ManufacturerSpecific = 0x72,
        Battery = 0x80,
        WakeUp = 0x84,
        Association = 0x85,
        Version = 0x86,
        SensorAlarm = 0x9C,
    }
}
