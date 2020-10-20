namespace ZWave.CommandClasses
{
    public enum ScheduleOverrideType : byte
    {
        NoOverride = 0x00,
        Advance = 0x01,
        RunForever = 0x02
    }
}
