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

        public async Task Set(byte parameter, sbyte value)
        {
            await Set(parameter, value, true);
        }

        public async Task Set(byte parameter, byte value)
        {
            await Set(parameter, value, false);
        }

        public async Task Set(byte parameter, short value)
        {
            await Set(parameter, value, true);
        }

        public async Task Set(byte parameter, ushort value)
        {
            await Set(parameter, value, false);
        }

        public async Task Set(byte parameter, int value)
        {
            await Set(parameter, value, true);
        }

        public async Task Set(byte parameter, uint value)
        {
            await Set(parameter, value, false);
        }

        public async Task Set(byte parameter, long value)
        {
            await Set(parameter, value, true);
        }

        public async Task Set(byte parameter, ulong value)
        {
            await Set(parameter, (long)value, false);
        }

        private async Task Set(byte parameter, long value, bool signed)
        {
            // extra roundtrip to get the correct size of the parameter
            var response = await Channel.Send(Node, new Command(Class, command.Get, parameter), command.Report);
            var size = response[1];

            var values = default(byte[]);
            switch(size)
            {
                case 1:
                    values = signed ? PayloadConverter.GetBytes((sbyte)value) : PayloadConverter.GetBytes((byte)value);
                    break;
                case 2:
                    values = signed ? PayloadConverter.GetBytes((short)value) : PayloadConverter.GetBytes((ushort)value);
                    break;
                case 4:
                    values = signed ? PayloadConverter.GetBytes((int)value) : PayloadConverter.GetBytes((uint)value);
                    break;
                case 8:
                    values = signed ? PayloadConverter.GetBytes((long)value) : PayloadConverter.GetBytes((ulong)value);
                    break;
                default:
                    throw new NotSupportedException($"Size:{size} is not supported");
            }
            await Channel.Send(Node, new Command(Class, command.Set, new[] { parameter, (byte)values.Length }.Concat(values).ToArray()));
        }
    }
}
