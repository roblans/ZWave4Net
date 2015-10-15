using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.CommandClasses
{
    public enum ThermostatSetpointType : byte
    {
        Unused = 0x00,
        Heating = 0x01,
        Cooling = 0x02,
        Unused3 = 0x03,
        Unused4 = 0x04,
        Unused5 = 0x05,
        Unused6 = 0x06,
        Furnace = 0x07,
        DryAir = 0x08,
        MoistAir = 0x09,
        AutoChangeover = 0x0A,
        HeatingEcon = 0x0B,
        CoolingEcon = 0x0C,
        AwayHeating = 0x0D,
        Count = 0x0E
    };
}
