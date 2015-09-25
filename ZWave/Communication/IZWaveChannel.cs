using System;
using System.Threading.Tasks;

namespace ZWave.Communication
{
    public interface IZWaveChannel
    {
        void Open();
        void Close();

        Task<byte[]> Send(Function function, params byte[] payload);
        Task Send(byte nodeID, Command command);
        Task<byte[]> Send(byte nodeID, Command command, byte responseCommandID);

        event EventHandler<NodeEventArgs> NodeEventReceived;
    }

    public static partial class Extentions
    {
        public static Task<byte[]> Send(this IZWaveChannel channel, byte nodeID, Command command, Enum responseCommand)
        {
            return channel.Send(nodeID, command, Convert.ToByte(responseCommand));
        }
    }
}