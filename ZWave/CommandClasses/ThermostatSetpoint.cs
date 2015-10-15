using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class ThermostatSetpoint : CommandClassBase
    {
        enum command : byte
        {
            Set = 0x01,
            Get = 0x02,
            Report = 0x03
        }

        public ThermostatSetpoint(Node node)
            : base(node, CommandClass.ThermostatSetpoint)
        {
        }

        public async Task<ThermostatSetpointReport> Get(ThermostatSetpointType type)
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get, Convert.ToByte(type)), command.Report);
            return new ThermostatSetpointReport(Node, response);
        }

        public async Task Set(byte value)
        {
            await Channel.Send(Node, new Command(Class, command.Set, value));
        }

    }
}
