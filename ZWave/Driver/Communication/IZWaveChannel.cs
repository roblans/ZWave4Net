using System;
using System.Threading.Tasks;

namespace ZWave.Driver.Communication
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
}