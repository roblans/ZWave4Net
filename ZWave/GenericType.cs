using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave
{
    public enum GenericType : byte
    {
        Unknown = 0x00,
        PortableRemote = 0x01,
        StaticController = 0x02,
        AVControlPoint = 0x03,
        RoutingSlave = 0x04,
        Display = 0x06,
        GarageDoor = 0x07,
        Thermostat = 0x08,
        WindowCovering = 0x09,
        RepeaterSlave = 0x0F,
        SwitchBinary = 0x10,
        SwitchMultiLevel = 0x11,
        SwitchRemote = 0x12,
        SwitchToggle = 0x13,
        SensorBinary = 0x20,
        SensorMultiLevel = 0x21,
        WaterControl = 0x22,
        MeterPulse = 0x30,
        EntryControl = 0x40,
        SemiInteroperable = 0x50,
        SmokeDetector = 0xA1,
        NonInteroperable = 0xFF
    }
}
