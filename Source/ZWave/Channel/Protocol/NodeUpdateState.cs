using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Channel.Protocol
{
    public enum NodeUpdateState
    {
        NodeInfoReceived = 0x84,
        NodeInfoRequestDone = 0x82,
        NodeInfoRequestFailed = 0x81,
        RoutingPending = 0x80,
        NewIdAssigned = 0x40,
        DeletedDone = 0x20,
        SucId = 0x10,
    }
}
