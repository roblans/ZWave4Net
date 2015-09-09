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
            await DiscoverNodes();
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

        public Task DiscoverNodes()
        {
            return _nodes = Task.Run(async () =>
            {
                var response = await Channel.Send(Function.DiscoveryNodes);
                var values = response.Payload.Skip(3).Take(29).ToArray();

                var nodes = new List<Node>();
                var bits = new BitArray(values);
                for (byte i = 0; i < bits.Length; i++)
                {
                    if (bits[i])
                    {
                        var node = new Node((byte)(i + 1), this);
                        nodes.Add(node);
                    }
                }
                return nodes.ToArray();
            });
        }

        //public async Task<byte[]> SendNodeInformation(Node node)
        //{
        //    var response = await Channel.Send(Function.SendNodeInformation, node);
        //    return response.Payload;
        //}

        //public async Task<byte[]> RequestNodeInfo(Node node)
        //{
        //    var response = await Channel.Send(Function.RequestNodeInfo, node);
        //    return response.Payload;
        //}

        //public async Task<byte[]> SerialApiApplNodeInformation(Node node)
        //{
        //    var response = await Channel.Send(Function.SerialApiApplNodeInformation, node);
        //    return response.Payload;
        //}

        public async Task<Node[]> GetNodes()
        {
            return _nodes != null ? await _nodes : new Node[0];
        }

        public async Task Close()
        {
            await Channel.Close();
        }
    }
}
