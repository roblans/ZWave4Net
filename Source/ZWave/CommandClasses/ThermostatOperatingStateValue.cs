namespace ZWave.CommandClasses
{
    public enum ThermostatOperatingStateValue : byte
    {
        Idle = 0x00,
        Heating = 0x01,
        Cooling = 0x02,
        FanOnly = 0x03,
        PendingHeat = 0x04,
        PendingCool = 0x05,
        VentEconomizer = 0x06,
        AuxHeating = 0x07,
        SecondStageHeating = 0x08,
        SecondStageCooling = 0x09,
    };
}

