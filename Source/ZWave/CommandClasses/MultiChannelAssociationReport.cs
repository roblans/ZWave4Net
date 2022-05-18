using System;
using System.Linq;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class MultiChannelAssociationReport : NodeReport
    {
        private const byte MultiChannelAssociationReportMarker = 0;

        public byte GroupID { get; }

        public byte MaxNodesSupported { get; }

        public byte ReportsToFollow { get; }

        public byte[] Nodes { get; }

        public MultiChannelAssociation.Endpoint[] Endpoints { get; }

        internal MultiChannelAssociationReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 3)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            GroupID = payload[0];
            MaxNodesSupported = payload[1];
            ReportsToFollow = payload[2];
            Nodes = payload.Skip(3).TakeWhile(b => b != MultiChannelAssociationReportMarker).ToArray();
            byte[] endpointsPayload = payload.Skip(3).SkipWhile(b => b != MultiChannelAssociationReportMarker).Skip(1).ToArray();
            if (endpointsPayload.Length % 2 != 0)
            {
                throw new ReponseFormatException($"The response was not in the expected format. Wrong format of endpoints. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");
            }

            Endpoints = new MultiChannelAssociation.Endpoint[endpointsPayload.Length / 2];
            for (int i = 0, j = 0; i < Endpoints.Length; i++, j+=2)
            {
                Endpoints[i] = new MultiChannelAssociation.Endpoint(endpointsPayload[j], endpointsPayload[j + 1]);
            }
        }
    }
}
