using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.Controller.CommandClasses
{
    public class Configuration : CommandClassBase
    {
        enum command : byte
        {
            Set = 0x04,
            Get = 0x05,
            Report = 0x06
        }

        public Configuration(Node node) : base(node, CommandClass.Configuration)
        {
        }

        public async Task<ConfigurationReport> Get(byte parameter)
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get, parameter), command.Report);
            return new ConfigurationReport(Node, response);
        }


        public async Task Set(byte parameter, sbyte value)
        {
            await Set(parameter, (long)value);
        }

        public async Task Set(byte parameter, short value)
        {
            await Set(parameter, (long)value);
        }

        public async Task Set(byte parameter, int value)
        {
            await Set(parameter, (long)value);
        }

        public async Task Set(byte parameter, long value)
        {
            // extra roundtrip to get the correct size of the parameter
            var response = await Channel.Send(Node, new Command(Class, command.Get, parameter), command.Report);
            var size = response[1];

            // adjust payload to correct size
            var values = PayloadConverter.GetBytes(value);
            values = values.Skip(values.Length - size).Take(size).ToArray();

            await Channel.Send(Node, new Command(Class, command.Set, new[] { parameter, (byte)values.Length }.Concat(values).ToArray()));
        }
    }
}
