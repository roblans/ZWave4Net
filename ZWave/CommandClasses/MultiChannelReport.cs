using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            if (payload.Length < 3)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            ControllerID = payload[0];
            EndPointID = payload[1];

            // check sub report
            if(payload.Length > 3 && payload[2] == 37 && payload[3] == 3)
            {
                Report = new SwitchBinaryReport(node, payload.Skip(4).ToArray<Byte>());
            }
        }

        public override string ToString()
        {
            return $"ControllerID:{ControllerID}. EndPointID:{EndPointID}. Report:{Report}";
        }
    }
}
