using System;
using System.IO;
using System.Threading.Tasks;

namespace ZWave.Channel
{
    public interface IZWaveChannel
    {
        TextWriter Log { get; set; }

        void Open();
        void Close();

        Task<byte[]> Send(Function function, params byte[] payload);
        Task Send(byte nodeID, Command command);
        Task<byte[]> Send(byte nodeID, Command command, byte responseCommandID);

        event EventHandler<NodeEventArgs> NodeEventReceived;
    }
}