using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ZWave.Channel;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class MultiChannelReport : NodeReport
    {
        public readonly byte ControllerID;
        public readonly byte EndPointID;
        public readonly NodeReport Report;

        internal MultiChannelReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 1)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            ControllerID = payload[0];
            EndPointID = payload[1];
            
            if (byte.Equals(payload[2], (byte)CommandClass.SwitchBinary) && byte.Equals(payload[3], (byte)SwitchBinary.command.Report))
                Report = new SwitchBinaryReport(node, payload.Skip(2 + 2).ToArray());
        }

        public override string ToString()
        {
            return $"ControllerID:{ControllerID}, EndPointID:{EndPointID}, Report:{Report.ToString()}";
        }
    }
}
