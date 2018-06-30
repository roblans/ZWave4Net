using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Channel;
using System.Threading;

namespace ZWave.CommandClasses
{
    public class Association : CommandClassBase
    {
        enum command
        {
            Set = 0x01,
            Get = 0x02,
            Report = 0x03,
            Remove = 0x04,
            GroupingsGet = 0x05,
            GroupingsReport = 0x06
        }

        public Association(Node node) : base(node, CommandClass.Association)
        {
        }

        public Task<AssociationReport> Get(byte groupID)
        {
            return Get(groupID, CancellationToken.None);
        }

        public async Task<AssociationReport> Get(byte groupID, CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get, groupID), command.Report, cancellationToken);
            return new AssociationReport(Node, response);
        }

        public Task Add(byte groupID, params byte[] nodes)
        {
            return Add(groupID, CancellationToken.None, nodes);
        }

        public async Task Add(byte groupID, CancellationToken cancellationToken, params byte[] nodes)
        {
            var payload = new byte[] { groupID }.Concat(nodes).ToArray();
            await Channel.Send(Node, new Command(Class, command.Set, payload), cancellationToken);
        }

        public Task Remove(byte groupID, params byte[] nodes)
        {
            return Remove(groupID, CancellationToken.None, nodes);
        }

        public async Task Remove(byte groupID, CancellationToken cancellationToken, params byte[] nodes)
        {
            var payload = new byte[] { groupID }.Concat(nodes).ToArray();
            await Channel.Send(Node, new Command(Class, command.Remove, payload), cancellationToken);
        }

        public Task<AssociationGroupsReport> GetGroups()
        {
            return GetGroups(CancellationToken.None);
        }

        public async Task<AssociationGroupsReport> GetGroups(CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.GroupingsGet), command.GroupingsReport, cancellationToken);
            return new AssociationGroupsReport(Node, response);
        }
    }
}
