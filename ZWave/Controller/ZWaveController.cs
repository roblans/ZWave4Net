using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave.Communication;

namespace ZWave.Controller
{
    public class ZWaveContoller
    {
        private Task<NodeCollection> _getNodes;
        public readonly ZWaveChannel Channel;

        private ZWaveContoller(ZWaveChannel channel)
        {
            Channel = channel;
        }

        public ZWaveContoller(ISerialPort port)
            : this(new ZWaveChannel(port))
        {
        }

#if NET || WINDOWS_UWP
        public ZWaveContoller(string portName)
            : this(new ZWaveChannel(portName))
        {
        }
#endif

        public void Open()
        {
            Channel.NodeEventReceived += Channel_NodeEventReceived;
            Channel.Open();
        }

        private async void Channel_NodeEventReceived(object sender, NodeEventArgs e)
        {
            var nodes = await GetNodes();
            var target = nodes[e.NodeID];
            if (target != null)
            {
                target.HandleEvent(e.Command);
            }
        }

        public void Close()
        {
            Channel.NodeEventReceived -= Channel_NodeEventReceived;
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
            return PayloadConverter.ToUInt32(response);
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
