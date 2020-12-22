namespace ZWave.CommandClasses
{
    public enum ThermostatFanModeValue : byte
    {
        AutoLow = 0x00,
        Low = 0x01,
        AutoHigh = 0x02,
        High = 0x03,
        AutoMedium = 0x04,
        Medium = 0x05,
        Circulation = 0x06,
        HumidityCirculation = 0x07,
        LeftAndRight = 0x08,
        UpAndDown = 0x09,
        Quiet = 0x0A,
        ExternalCirculation = 0x0B,
        EnergyCool = 0x0C,
        Away = 0x0D,
        Reserved = 0x0E,
        FullPower = 0x0F,
        ManufacturerSpecific = 0x1F
    };
}

