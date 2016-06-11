using System;
using System.Collections.Generic;
using System.Text;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class SceneActivationReport : NodeReport
    {
        public readonly byte SceneID;
        public readonly TimeSpan? Time;

        internal SceneActivationReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 2)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            SceneID = payload[0];
            if (payload[1] == 0)
            {
                Time = TimeSpan.Zero;
            }
            else if (payload[1] <= 0x7F)
            {
                Time = TimeSpan.FromSeconds(payload[1]);
            }
            else if (payload[1] <= 0xFE)
            {
                Time = TimeSpan.FromMilliseconds(payload[1]);
            }
            else
            {
                Time = null; // via configuration
            }
        }

        public override string ToString()
        {
            return $"SceneID:{SceneID}, Time:{Time}";
        }
    }
}
