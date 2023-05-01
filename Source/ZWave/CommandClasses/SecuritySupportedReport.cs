using System;
using System.Collections.Generic;
using ZWave.Channel;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class SecuritySupportedReport : NodeReport
    {
        private const byte COMMAND_CLASS_MARK = 0xEF;
        public readonly CommandClass[] Classes;

        internal SecuritySupportedReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 2)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");
            List<CommandClass> classes = new List<CommandClass>();
            for (int i = 1; i < payload.Length; i++)
            {
                if (payload[i] == COMMAND_CLASS_MARK)
                    break;
                classes.Add((CommandClass)payload[i]);
            }
        }

        public override string ToString()
        {
            return $"Supported:{String.Join(",", Classes)}";
        }
    }
}
