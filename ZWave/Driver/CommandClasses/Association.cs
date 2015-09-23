using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Driver.Communication;

namespace ZWave.Driver.CommandClasses
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

        public async Task<BatteryReport> Get()
        {
            var response = await Channel.Send(Node, new Command(Class, command.Get), command.Report);
            return new BatteryReport(Node, response);
        }

        public async Task Add(byte groupID, byte nodeID)
        {
            await Channel.Send(Node, new Command(Class, command.Set, groupID, nodeID));
        }

        public async Task Remove(byte groupID, byte nodeID)
        {
            await Channel.Send(Node, new Command(Class, command.Remove, groupID, nodeID));
        }
    }
}
