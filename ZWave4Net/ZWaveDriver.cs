using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave4Net.Communication;

namespace ZWave4Net
{
    public class ZWaveDriver
    {
        private Task<Node[]> _nodes;
        internal readonly MessageChannel Channel;

        public ZWaveDriver(ISerialPort port)
        {
            Channel = new MessageChannel(port);
        }

        public async Task Open()
        {
            Channel.Open();
            Platform.Log(LogLevel.Info, string.Format("Version: {0}", await GetVersion()));
            Platform.Log(LogLevel.Info, string.Format("HomeID: {0:X}", await GetHomeID()));

            DiscoverNodes();
            foreach (var node in await GetNodes())
            {
                Platform.Log(LogLevel.Info, string.Format("Discovered: Node = {0}, {1}", node, await GetNodeProtocolInfo(node)));
                //Log(string.Format("NodeInformation: {0}", BitConverter.ToString(await SendNodeInformation(node))));
                //Log(string.Format("RequestNodeInfo: {0}", BitConverter.ToString(await RequestNodeInfo(node))));SerialApiApplNodeInformation
                //Log(string.Format("SerialApiApplNodeInformation: {0}", BitConverter.ToString(await SerialApiApplNodeInformation(node))));
            }
        }


        public async Task<string> GetVersion()
        {
            var response = await Channel.Send(Function.GetVersion);
            var data = response.Payload.TakeWhile(element => element != 0).ToArray();
            return Encoding.UTF8.GetString(data, 0, data.Length);
        }

        public async Task<uint> GetHomeID()
        {
            var response = await Channel.Send(Function.MemoryGetId);
            return BitConverter.ToUInt32(response.Payload.Take(4).Reverse().ToArray(), 0);
        }

        public void DiscoverNodes()
        {
            _nodes = Task.Run(async () =>
            {
                var response = await Channel.Send(Function.DiscoveryNodes);
                var values = response.Payload.Skip(3).Take(29).ToArray();

                var nodes = new List<Node>();
                var bits = new BitArray(values);
                for (byte i = 0; i < bits.Length; i++)
                {
                    if (bits[i])
                    {
                        nodes.Add(new Node((byte)(i + 1), this));
                    }
                }
                return nodes.ToArray();
            });
        }

        public async Task<NodeProtocolInfo> GetNodeProtocolInfo(Node node)
        {
            var response = await Channel.Send(Function.GetNodeProtocolInfo, node);
            return new NodeProtocolInfo(response.Payload);
        }

        public async Task<byte[]> SendNodeInformation(Node node)
        {
            var response = await Channel.Send(Function.SendNodeInformation, node);
            return response.Payload;
        }

        public async Task<byte[]> RequestNodeInfo(Node node)
        {
            var response = await Channel.Send(Function.RequestNodeInfo, node);
            return response.Payload;
        }

        public async Task<byte[]> SerialApiApplNodeInformation(Node node)
        {
            var response = await Channel.Send(Function.SerialApiApplNodeInformation, node);
            return response.Payload;
        }

        public async Task<Node[]> GetNodes()
        {
            return _nodes != null ? await _nodes : new Node[0];
        }

        public void Close()
        {
            Channel.Close(true);
        }
    }
}
