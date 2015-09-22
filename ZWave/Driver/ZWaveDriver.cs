using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave.Driver.Communication;

namespace ZWave.Driver
{
    public class ZWaveDriver
    {
        private Task<NodeCollection> _getNodes;
        public readonly ZWaveChannel Channel;

        private ZWaveDriver(ZWaveChannel channel)
        {
            Channel = channel;
        }

        public ZWaveDriver(ISerialPort port)
            : this(new ZWaveChannel(port))
        {
        }

#if NET || WINDOWS_UWP
        public ZWaveDriver(string portName)
            : this(new ZWaveChannel(portName))
        {
        }
#endif

        public void Open()
        {
            Channel.Open();
        }

        public void Close()
        {
            Channel.Close();
        }

        public async Task<string> GetVersion()
        {
            var response = await Channel.Send(Function.GetVersion);
            var data = response.TakeWhile(element => element != 0).ToArray();
            return Encoding.UTF8.GetString(data, 0, data.Length);
        }

        public async Task<uint> GetHomeID()
        {
            var response = await Channel.Send(Function.MemoryGetId);
            return ByteConverter.ToUInt32(response);
        }

        public async Task<byte> GetContollerID()
        {
            var response = await Channel.Send(Function.MemoryGetId);
            return response[4];
        }

        public Task<NodeCollection> DiscoverNodes()
        {
            return _getNodes = Task.Run(async () =>
            {
                var response = await Channel.Send(Function.DiscoveryNodes);
                var values = response.Skip(3).Take(29).ToArray();

                var nodes = new NodeCollection();
                var bits = new BitArray(values);
                for (byte i = 0; i < bits.Length; i++)
                {
                    if (bits[i])
                    {
                        var node = new Node((byte)(i + 1), Channel);
                        nodes.Add(node);
                    }
                }
                return nodes;
            });
        }

        public async Task<NodeCollection> GetNodes()
        {
            return await (_getNodes ?? (_getNodes = DiscoverNodes()));
        }
    }
}
