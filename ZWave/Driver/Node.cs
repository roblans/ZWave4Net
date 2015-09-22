using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<NodeProtocolInfo> GetNodeProtocolInfo()
        {
            var response = await Channel.Send(Function.GetNodeProtocolInfo, NodeID);
            return NodeProtocolInfo.Parse(response);
        }

        public override string ToString()
        {
            return $"{NodeID:D3}";
        }
    }
}
