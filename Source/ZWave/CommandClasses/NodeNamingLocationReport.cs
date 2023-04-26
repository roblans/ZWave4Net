using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZWave.Channel;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class NodeNamingLocationReport : NodeReport
    {
        public readonly string Location;

        internal NodeNamingLocationReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 1)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            if ((payload[0] & 0x3) < 0x2)
                Location = Encoding.ASCII.GetString(payload.Skip(1).Take(16).ToArray());
            else
                Location = Encoding.Unicode.GetString(payload.Skip(1).Take(16).ToArray());
        }

        public override string ToString()
        {
            return $"Location: {Location}";
        }
    }
}
