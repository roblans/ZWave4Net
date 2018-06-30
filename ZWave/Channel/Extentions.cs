using System;
using System.Threading;
using System.Threading.Tasks;

namespace ZWave.Channel
{
    public static partial class Extentions
    {
        public static Task<byte[]> Send(this ZWaveChannel channel, byte nodeID, Command command, Enum responseCommand)
        {
            return channel.Send(nodeID, command, Convert.ToByte(responseCommand), CancellationToken.None);
        }

        public static Task<byte[]> Send(this ZWaveChannel channel, byte nodeID, Command command, Enum responseCommand, CancellationToken cancellationToken)
        {
            return channel.Send(nodeID, command, Convert.ToByte(responseCommand), cancellationToken);
        }

        public static Task Send(this ZWaveChannel channel, Node node, Command command)
        {
            return channel.Send(node.NodeID, command, CancellationToken.None);
        }

        public static Task Send(this ZWaveChannel channel, Node node, Command command, CancellationToken cancellationToken)
        {
            return channel.Send(node.NodeID, command, cancellationToken);
        }

        public static Task<byte[]> Send(this ZWaveChannel channel, Node node, Command command, Enum responseCommand)
        {
            return channel.Send(node.NodeID, command, Convert.ToByte(responseCommand), CancellationToken.None);
        }

        public static Task<byte[]> Send(this ZWaveChannel channel, Node node, Command command, Enum responseCommand, CancellationToken cancellationToken)
        {
            return channel.Send(node.NodeID, command, Convert.ToByte(responseCommand), cancellationToken);
        }

        public static Task<byte[]> Send(this ZWaveChannel channel, Node node, Command command, Enum responseCommand, Func<byte[], bool> payloadValidation)
        {
            return channel.Send(node.NodeID, command, Convert.ToByte(responseCommand), payloadValidation, CancellationToken.None);
        }

        public static Task<byte[]> Send(this ZWaveChannel channel, Node node, Command command, Enum responseCommand, Func<byte[], bool> payloadValidation, CancellationToken cancellationToken)
        {
            return channel.Send(node.NodeID, command, Convert.ToByte(responseCommand), payloadValidation, cancellationToken);
        }
    }
}
