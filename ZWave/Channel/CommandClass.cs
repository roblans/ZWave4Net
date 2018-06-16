namespace ZWave.Channel
{
    public enum CommandClass : byte
    {
        NoOperation = 0x00,
        Basic = 0x20,
        SwitchBinary = 0x25,
        SwitchMultiLevel = 0x26,
        SceneActivation = 0x2B,
        SensorBinary = 0x30,
        SensorMultiLevel = 0x31,
        Meter = 0x32,
        Color = 0x33,
        ThermostatSetpoint = 0x43,
        CentralScene = 0x5B,
        MultiChannel = 0x60,
        Configuration = 0x70,
        Alarm = 0x71,
        ManufacturerSpecific = 0x72,
        Battery = 0x80,
        Clock = 0x81,
        WakeUp = 0x84,
        Association = 0x85,
        Version = 0x86,
        MultiChannelAssociation = 0x8E,
        SensorAlarm = 0x9C,
    }
}
