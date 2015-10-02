using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
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

        public async Task Set(byte parameter, long value)
        {
            // extra roundtrip to get the correct size of the parameter
            var response = await Channel.Send(Node, new Command(Class, command.Get, parameter), command.Report);
            var size = response[1];

            var values = default(byte[]);
            switch(size)
            {
                case 1:
                    if (value >= 0)
                    {
                        values = PayloadConverter.GetBytes((byte)value);
                    }
                    else
                    {
                        values = PayloadConverter.GetBytes((sbyte)value);
                    }
                    break;
                case 2:
                    if (value >= 0)
                    {
                        values = PayloadConverter.GetBytes((ushort)value);
                    }
                    else
                    {
                        values = PayloadConverter.GetBytes((short)value);
                    }
                    break;
                case 4:
                    if (value >= 0)
                    {
                        values = PayloadConverter.GetBytes((uint)value);
                    }
                    else
                    {
                        values = PayloadConverter.GetBytes((int)value);
                    }
                    break;
                case 8:
                    if (value >= 0)
                    {
                        values = PayloadConverter.GetBytes((ulong)value);
                    }
                    else
                    {
                        values = PayloadConverter.GetBytes(value);
                    }
                    break;
            }
            await Channel.Send(Node, new Command(Class, command.Set, new[] { parameter, (byte)values.Length }.Concat(values).ToArray()));
        }
    }
}
