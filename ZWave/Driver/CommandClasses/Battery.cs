using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Driver.Communication;

namespace ZWave.Driver.CommandClasses
{
    public class Battery : CommandClassBase
    {
        enum command
        {
            Get = 0x02,
            Report = 0x03
        }

        public Battery(Node node) : base(node, CommandClass.Battery)
        {
        }

        public async Task<BatteryReport> Get()
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get), command.Report);
            return new BatteryReport(Node, response);
        }
    }
}
