using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class Color : CommandClassBase
    {
        enum command
        {
            Get = 0x03,
            Report = 0x04,
            Set = 0x05,
        }

        public Color(Node node) : base(node, CommandClass.Color)
        {
        }

        public async Task Set(ColorComponent[] components)
        {
            var payload = new List<byte>();
            payload.Add((byte)components.Length);
            payload.AddRange(components.SelectMany(element => element.ToBytes()));
            await Channel.Send(Node, new Command(Class, command.Set, payload.ToArray()));
        }

        public async Task<ColorReport> Get(byte componentID)
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get, componentID), command.Report);
            return new ColorReport(Node, response);
        }
    }
}
