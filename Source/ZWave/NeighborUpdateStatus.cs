using System;
using System.Collections.Generic;
using System.Text;
using ZWave.Channel.Protocol;
using ZWave.CommandClasses;

namespace ZWave
{
    public enum NeighborUpdateStatus
    {
        Started = 0x21,
        Done = 0x22,
        Failed = 0x23,
    }
}
