using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Channel
{
    public static partial class Extentions
    {
        public static Task<byte[]> Send(this ZWaveChannel channel, byte nodeID, Command command, Enum responseCommand)
        {
            return channel.Send(nodeID, command, Convert.ToByte(responseCommand));
        }
        public static Task Send(this ZWaveChannel channel, Node node, Command command)
        {
            return channel.Send(node.NodeID, command);
        }

        public static Task<byte[]> Send(this ZWaveChannel channel, Node node, Command command, Enum responseCommand)
        {
            return channel.Send(node.NodeID, command, Convert.ToByte(responseCommand));
        }
    }
}
