using System;
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
            if (payload.Length < 3)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            EndPointID = payload[0];
            ControllerID = payload[1];

            // check sub report
            if (payload.Length > 3 && payload[2] == Convert.ToByte(CommandClass.SwitchBinary) && payload[3] == Convert.ToByte(SwitchBinary.command.Report))
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
