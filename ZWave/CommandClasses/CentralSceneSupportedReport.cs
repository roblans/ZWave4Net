using System;
using System.Collections.Generic;
using System.Text;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class CentralSceneSupportedReport : NodeReport
    {
        public readonly byte SceneCount;

        internal CentralSceneSupportedReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 1)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            SceneCount = payload[0];
        }

        public override string ToString()
        {
            return $"Scene:{SceneCount}";
        }
    }
}
