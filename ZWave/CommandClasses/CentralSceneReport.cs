using System;
using System.Collections.Generic;
using System.Text;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class CentralSceneReport : NodeReport
    {
        public readonly byte Scene;

        internal CentralSceneReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 3)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            Scene = payload[2];
        }

        public override string ToString()
        {
            return $"Scene:{Scene}";
        }
    }
}
