using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.Controller.CommandClasses
{
    public class Version : CommandClassBase
    {
        enum command : byte
        {
            Get = 0x11,
            Report = 0x12,
            CommandClassGet = 0x13,
            CommandClassReport = 0x14
        }


        public Version(Node node) : base(node, CommandClass.Version)
        {
        }

        public async Task<VersionReport> Get()
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get), command.Report);
            return new VersionReport(Node, response);
        }

        public async Task<VersionCommandClassReport> GetCommandClass(CommandClass @class)
        {
            var response = await Channel.Send(Node, new Command(Class, command.CommandClassGet, Convert.ToByte(@class)), command.CommandClassReport);
            return new VersionCommandClassReport(Node, response);
        }
    }
}
