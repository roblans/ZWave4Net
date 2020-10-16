namespace ZWave.CommandClasses
{
    public enum ThermostatModeValue : byte
    {
        Off = 0x00,
        Heat = 0x01,
        Cool = 0x02,
        Auto = 0x03,
        Auxiliary = 0x04,
        Resume = 0x05,
        Fan = 0x06,
        Furnace = 0x07,
        Dry = 0x08,
        Moist = 0x09,
        AutoChangeover = 0x0A,
        EnergyHeat = 0x0B,
        EnergyCool = 0x0C,
        Away = 0x0D,
        Reserved = 0x0E,
        FullPower = 0x0F,
        ManufacturerSpecific = 0x1F
    };
}

