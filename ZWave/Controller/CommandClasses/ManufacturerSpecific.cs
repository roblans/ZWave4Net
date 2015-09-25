using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Communication;

namespace ZWave.Controller.CommandClasses
{
    public class ManufacturerSpecific : CommandClassBase
    {
        enum command
        {
            Get = 0x04,
            Report = 0x05
        }

        public ManufacturerSpecific(Node node) : base(node, CommandClass.ManufacturerSpecific)
        {
        }

        public async Task<ManufacturerSpecificReport> Get()
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get), command.Report);
            return new ManufacturerSpecificReport(Node, response);
        }
    }
}
