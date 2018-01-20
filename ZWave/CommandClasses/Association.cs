using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Channel;

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

        public async Task<AssociationReport> Get(byte groupID)
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get, groupID), command.Report);
            return new AssociationReport(Node, response);
        }

        public async Task Add(byte groupID, params byte[] nodes)
        {
            var payload = new byte[] { groupID }.Concat(nodes).ToArray();
            await Channel.Send(Node, new Command(Class, command.Set, payload));
        }

        public async Task Remove(byte groupID, params byte[] nodes)
        {
            var payload = new byte[] { groupID }.Concat(nodes).ToArray();
            await Channel.Send(Node, new Command(Class, command.Remove, payload));
        }

        public async Task<AssociationGroupsReport> GetGroups()
        {
            var response = await Channel.Send(Node, new Command(Class, command.GroupingsGet), command.GroupingsReport);
            return new AssociationGroupsReport(Node, response);
        }
    }
}
