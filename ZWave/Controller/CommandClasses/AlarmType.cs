using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Controller.CommandClasses
{
    public enum AlarmType : byte
    {
        General = 0x00,
        Smoke = 0x01,
        CarbonMonoxide = 0x02,
        CarbonDioxide = 0x03,
        Heat = 0x04,
        Flood = 0x05,
        AccessControl = 0x06,
        Burglar = 0x07,
        PowerManagement = 0x08,
        System = 0x09,
        Emergency = 0x0A,
        Count = 0x0B,
    };
}
