using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
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
            Report = 0x06,
            BulkSet = 0x07,
            BulkGet = 0x08,
            BulkReport = 0x09
        }

        public Configuration(Node node) : base(node, CommandClass.Configuration)
        {
        }

        public Task<ConfigurationReport> Get(byte parameter)
        {
            return Get(parameter, CancellationToken.None);
        }

        public async Task<ConfigurationReport> Get(byte parameter, CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get, parameter), command.Report, cancellationToken);
            return new ConfigurationReport(Node, response);
        }

        public Task SetDefault(byte parameter)
        {
            return Set(parameter, 0, false, 0, CancellationToken.None, true);
        }

        public async Task SetDefault(byte parameter, CancellationToken cancellationToken)
        {
            await Set(parameter, 0, false, 0, cancellationToken, true);
        }

        public Task Set(byte parameter, sbyte value)
        {
            return Set(parameter, value, CancellationToken.None);
        }

        public async Task Set(byte parameter, sbyte value, CancellationToken cancellationToken)
        {
            await Set(parameter, value, true, 0, cancellationToken);
        }

        public Task Set(byte parameter, byte value)
        {
            return Set(parameter, value, CancellationToken.None);
        }

        public async Task Set(byte parameter, byte value, CancellationToken cancellationToken)
        {
            await Set(parameter, value, false, 0, cancellationToken);
        }

        public Task Set(byte parameter, short value)
        {
            return Set(parameter, value, CancellationToken.None);
        }

        public async Task Set(byte parameter, short value, CancellationToken cancellationToken)
        {
            await Set(parameter, value, true, 0, cancellationToken);
        }

        public Task Set(byte parameter, ushort value)
        {
            return Set(parameter, value, CancellationToken.None);
        }

        public async Task Set(byte parameter, ushort value, CancellationToken cancellationToken)
        {
            await Set(parameter, value, false, 0, cancellationToken);
        }

        public Task Set(byte parameter, int value)
        {
            return Set(parameter, value, CancellationToken.None);
        }

        public async Task Set(byte parameter, int value, CancellationToken cancellationToken)
        {
            await Set(parameter, value, true, 0, cancellationToken);
        }

        public Task Set(byte parameter, uint value)
        {
            return Set(parameter, value, CancellationToken.None);
        }

        public async Task Set(byte parameter, uint value, CancellationToken cancellationToken)
        {
            await Set(parameter, (int)value, false, 0, cancellationToken);
        }

        private async Task Set(byte parameter, int value, bool signed, byte size, CancellationToken cancellationToken, bool reset = false)
        {
            if (size == 0)
            {
                // extra roundtrip to get the correct size of the parameter
                var response = await Channel.Send(Node, new Command(Class, command.Get, parameter), command.Report, cancellationToken);
                size = response[1];
            }

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
                    values = signed ? PayloadConverter.GetBytes(value) : PayloadConverter.GetBytes((uint)value);
                    break;
                default:
                    throw new NotSupportedException($"Size:{size} is not supported");
            }
            if (reset)
                size |= 0x80;
            await Channel.Send(Node, new Command(Class, command.Set, new[] { parameter, size }.Concat(values).ToArray()), cancellationToken);
        }
    }
}
