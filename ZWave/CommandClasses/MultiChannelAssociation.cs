using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class MultiChannelAssociation : CommandClassBase
    {
        public class Endpoint
        {
            public byte NodeId { get; }

            public byte EndpointId { get; }

            public Endpoint(byte nodeId, byte endpointId)
            {
                NodeId = nodeId;
                EndpointId = endpointId;
            }
        }

        private const byte MultiChannelAssociationSetMarker = 0;
        private const byte MultiChannelAssociationRemoveMarker = 0;

        enum command
        {
            Set = 0x01,
            Get = 0x02,
            Report = 0x03,
            Remove = 0x04,
            GroupingsGet = 0x05,
            GroupingsReport = 0x06
        }

        public MultiChannelAssociation(Node node)
            : base(node, CommandClass.MultiChannelAssociation)
        { }

        public Task Add(byte groupId, byte[] destinationNodeIds, Endpoint[] destinationEndpoints)
        {
            return Channel.Send(Node, new Command(Class, command.Set, GetEndpointsPayload(groupId, destinationNodeIds, destinationEndpoints, MultiChannelAssociationSetMarker)));
        }

        public async Task<MultiChannelAssociationReport> Get(byte groupID)
        {
            byte[] response = await Channel.Send(Node, new Command(Class, command.Get, groupID), command.Report);
            return new MultiChannelAssociationReport(Node, response);
        }

        public Task Remove(byte groupId, byte[] destinationNodeIds, Endpoint[] destinationEndpoints)
        {
            return Channel.Send(Node, new Command(Class, command.Remove, GetEndpointsPayload(groupId, destinationNodeIds, destinationEndpoints, MultiChannelAssociationRemoveMarker)));
        }

        public async Task<AssociationGroupsReport> GetGroups()
        {
            var response = await Channel.Send(Node, new Command(Class, command.GroupingsGet), command.GroupingsReport);
            return new AssociationGroupsReport(Node, response);
        }

        private static byte[] GetEndpointsPayload(byte groupId, byte[] destinationNodeIds, Endpoint[] destinationEndpoints, byte marker)
        {
            IEnumerable<byte> endpointsPayload = destinationEndpoints.SelectMany(e => new byte[] { e.NodeId, e.EndpointId });
            return new byte[] { groupId }.Concat(destinationNodeIds).Concat(new byte[] { marker }).Concat(endpointsPayload).ToArray();
        }
    }
}
