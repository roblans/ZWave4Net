using System;
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

        public async Task Set(byte white, byte red, byte green, byte blue)
        {
            await Channel.Send(Node, new Command(Class, command.Set, 0x00, 0x00, 0x01, white, 0x02, red, 0x03, green, 0x04, blue));
        }

        public async Task<ColorReport> Get()
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get), command.Report);
            return new ColorReport(Node, response);
        }
    }
}
