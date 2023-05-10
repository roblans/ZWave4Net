using System;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class SwitchAllReport : NodeReport
    {
        public readonly SwitchAllMode Value;

        internal SwitchAllReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 1)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            Value = (SwitchAllMode)payload[0];
        }

        public override string ToString()
        {
            return $"Value:{Value}";
        }
    }
}
