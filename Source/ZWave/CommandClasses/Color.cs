using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZWave.Channel;
using System.Threading;
using ZWave.Channel.Protocol;
using System.Collections;

namespace ZWave.CommandClasses
{
    public class Color : CommandClassBase
    {
        enum command
        {
            SupportedGet = 0x01,
            SupportedReport = 0x02,
            Get = 0x03,
            Report = 0x04,
            Set = 0x05,
            StartLevelChange = 0x06,
            StopLevelChange = 0x07
        }

        public Color(Node node) : base(node, CommandClass.Color)
        {
        }

        public Task Set(ColorComponent[] components, TimeSpan? duration = null)
        {
            return Set(components, CancellationToken.None);
        }

        public async Task Set(ColorComponent[] components, CancellationToken cancellationToken, TimeSpan? duration = null)
        {
            var payload = new List<byte>();
            payload.Add((byte)Math.Min(components.Length, 31)); //31 Components max
            payload.AddRange(components.SelectMany(element => element.ToBytes()));
            if (duration != null)
                payload.Add(PayloadConverter.GetByte(duration.Value));
            await Channel.Send(Node, new Command(Class, command.Set, payload.ToArray()), cancellationToken);
        }

        public Task<ColorReport> Get(ColorComponentType component)
        {
            return Get(component, CancellationToken.None);
        }

        public async Task<ColorReport> Get(ColorComponentType component, CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get, (byte)component), command.Report, cancellationToken);
            return new ColorReport(Node, response);
        }

        public Task<ColorComponentType[]> GetSupported()
        {
            return GetSupported(CancellationToken.None);
        }

        public async Task<ColorComponentType[]> GetSupported(CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.SupportedGet), command.SupportedReport, cancellationToken);
            if (response.Length != 2)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(response)}");
            List<ColorComponentType> ret = new List<ColorComponentType>();
            BitArray bits = new BitArray(response);
            for (byte i = 0; i < bits.Length; i++)
            {
                if (bits[i])
                    ret.Add((ColorComponentType)i);
            }
            return ret.ToArray();
        }

        public Task StartLevelChange(bool up, ColorComponentType component, int startLevel)
        {
            return StartLevelChange(up, component, startLevel, CancellationToken.None);
        }

        public async Task StartLevelChange(bool up, ColorComponentType component, int startLevel, CancellationToken cancellationToken)
        {
            byte flags = 0x0;
            if (startLevel < 0)
                flags |= 0x20; //Ignore Start
            if (up)
                flags |= 0x40;
            await Channel.Send(Node, new Command(Class, command.StartLevelChange, flags, (byte)component, (byte)Math.Max(0, startLevel)), cancellationToken);
        }

        public Task StopLevelChange(ColorComponentType component)
        {
            return StopLevelChange(component, CancellationToken.None);
        }

        public async Task StopLevelChange(ColorComponentType component, CancellationToken cancellationToken)
        {
            await Channel.Send(Node, new Command(Class, command.StopLevelChange, (byte)component), cancellationToken);
        }
    }
}
