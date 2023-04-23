using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class NodeNaming : CommandClassBase
    {
        enum command : byte
        {
            SetName = 0x01,
            GetName = 0x02,
            ReportName = 0x03,
            SetLocation = 0x04,
            GetLocation = 0x05,
            ReportLocation = 0x06,
        }

        public NodeNaming(Node node) : base(node, CommandClass.NodeNaming)
        {
        }

        public Task<NodeNamingNameReport> GetName()
        {
            return GetName(CancellationToken.None);
        }

        public async Task<NodeNamingNameReport> GetName(CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.GetName), command.ReportName, cancellationToken);
            return new NodeNamingNameReport(Node, response);
        }


        public Task<NodeNamingLocationReport> GetLocation()
        {
            return GetLocation(CancellationToken.None);
        }

        public async Task<NodeNamingLocationReport> GetLocation(CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.GetLocation), command.ReportLocation, cancellationToken);
            return new NodeNamingLocationReport(Node, response);
        }

        public Task SetName(string name)
        {
            return Set(name, command.SetName, CancellationToken.None);
        }

        public Task SetName(string name, CancellationToken token)
        {
            return Set(name, command.SetName, CancellationToken.None);
        }

        public Task SetLocation(string name)
        {
            return Set(name, command.SetLocation, CancellationToken.None);
        }

        public Task SetLocation(string name, CancellationToken token)
        {
            return Set(name, command.SetLocation, CancellationToken.None);
        }

        private async Task Set(string txt, Enum command, CancellationToken cancellationToken)
        {
            byte encoding = 0;
            foreach (char c in txt)
            {
                if (c > 127)
                {
                    if (c <= 255)
                        encoding = 1;
                    else if (c > 255)
                    {
                        encoding = 2;
                        break;
                    }
                }
            }
            byte[] payload;
            if (encoding == 0)
                payload = Encoding.ASCII.GetBytes(txt);
            else if (encoding == 1)
                payload = Encoding.UTF8.GetBytes(txt);
            else
                payload = Encoding.Unicode.GetBytes(txt);
            payload = payload.Take(16).Prepend(encoding).ToArray();
            await Channel.Send(Node, new Command(Class, command, payload), cancellationToken);
        }
    }
}
