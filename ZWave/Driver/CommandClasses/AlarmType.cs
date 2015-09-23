using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Driver.CommandClasses
{
    public enum AlarmType : byte
    {
        Unknown = 0,
        Smoke = 1,
        CarbonMonoxide = 2,
        CarbonDioxide = 3,
        Heat = 4,
        Flood = 5,
        AccessControl = 6,
        Burglar = 7,
        PowerManagement = 8,
        System = 9,
        Emergency = 10,
        Clock = 11,
        First = 255,
    };
}
