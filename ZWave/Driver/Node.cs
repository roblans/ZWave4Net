using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZWave.Driver.CommandClasses;
using ZWave.Driver.Communication;

namespace ZWave.Driver
{
    public class Node
    {
        private List<ICommandClass> _commandClasses = new List<ICommandClass>();

        public readonly byte NodeID;
        public readonly ZWaveChannel Channel;

        public Node(byte nodeID, ZWaveChannel channel)
        {
            NodeID = nodeID;
            Channel = channel;

            _commandClasses.Add(new Basic(this));
        }
        
        public T GetCommandClass<T>()  where T : ICommandClass
        {
            return _commandClasses.OfType<T>().FirstOrDefault();
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
