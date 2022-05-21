using System;
namespace ZWave.CommandClasses
{
    public enum ScheduleState
    {
        NotUsed = 0x00,

        [Obsolete("A sending node SHOULD NOT use this status. It is RECOMMENDED to use 0x00 instead")]
        OverrideAndNotUsed = 0x01,

        NotActive = 0x02,

        Active = 0x03,

        Disabled = 0x04,

        OverrideAndActive = 0x05,

        [Obsolete("A sending node SHOULD NOT use this status. It is RECOMMENDED to use 0x02 instead")]
        OverrideAndNotActive = 0x06,

        [Obsolete("A sending node SHOULD NOT use this status. It is RECOMMENDED to use 0x04 instead")]
        OverrideAndDisabled = 0x07
    }
}
