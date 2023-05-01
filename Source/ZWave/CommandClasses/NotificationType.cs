namespace ZWave.CommandClasses
{
    public enum NotificationType : byte
    {
        General = 0x00,
        Smoke = 0x01,
        CarbonMonoxide = 0x02,
        CarbonDioxide = 0x03,
        Heat = 0x04,
        Flood = 0x05,
        AccessControl = 0x06,
        HomeSecurity = 0x07,
        PowerManagement = 0x08,
        System = 0x09,
        Emergency = 0x0A,
        Count = 0x0B,
        Clock = 0x0B,
        Appliance = 0x0C,
        HomeHealth = 0x0D,
        Siren = 0x0E,
        WaterValve = 0x0F,
        Weather = 0x10,
        Irrigation = 0x11,
        Gas = 0x12,
        Pest = 0x13,
        Light = 0x14,
        WaterQuality = 0x15,
        Home = 0x16,
        Unknown = 0xFE
    };
}
