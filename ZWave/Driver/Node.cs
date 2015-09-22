using System;
using System.Collections.Generic;
using System.Text;
using ZWave.Driver.Communication;

namespace ZWave.Driver
{
    public class Node
    {
        public readonly byte NodeID;
        public readonly ZWaveChannel Channel;

        public Node(byte nodeID, ZWaveChannel channel)
        {
            NodeID = nodeID;
            Channel = channel;
        }

        public override string ToString()
        {
            return $"{NodeID:D3}";
        }
    }
}
