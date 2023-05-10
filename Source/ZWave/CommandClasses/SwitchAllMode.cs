namespace ZWave.CommandClasses
{
    public enum SwitchAllMode : byte
    {
        ExcludeAll = 0x0,
        AllowAllOffOnly = 0x1,
        AllowAllOnOnly = 0x2,
        AllowAll = 0xFF
    }
}
