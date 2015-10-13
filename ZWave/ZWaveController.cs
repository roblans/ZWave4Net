using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave
{
    public class ZWaveController
    {
        private Task<NodeCollection> _getNodes;
        private string _version;
        private uint? _homeID;
        private byte? _nodeID;
        public readonly ZWaveChannel Channel;

        private ZWaveController(ZWaveChannel channel)
        {
            Channel = channel;
        }

        public ZWaveController(ISerialPort port)
            : this(new ZWaveChannel(port))
        {
        }

#if NET || WINDOWS_UWP
        public ZWaveController(string portName)
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
            if (_version == null)
            {
                var response = await Channel.Send(Function.GetVersion);
                var data = response.TakeWhile(element => element != 0).ToArray();
                _version = Encoding.UTF8.GetString(data, 0, data.Length);
            }
            return _version;
        }

        public async Task<uint> GetHomeID()
        {
            if (_homeID == null)
            {
                var response = await Channel.Send(Function.MemoryGetId);
                _homeID = PayloadConverter.ToUInt32(response);
            }
            return _homeID.Value;
        }

        public async Task<byte> GetNodeID()
        {
            if (_nodeID == null)
            {
                var response = await Channel.Send(Function.MemoryGetId);
                _nodeID = response[4];
            }
            return _nodeID.Value;
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
                        var node = new Node((byte)(i + 1), this);
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
