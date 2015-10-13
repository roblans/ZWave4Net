using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class MultiChannel : CommandClassBase
    {
        enum command
        {
            // version 2 only
            EndPointGet = 0x07,
            EndPointReport = 0x08,
            Encap = 0x0d,
        }

        public MultiChannel(Node node) : base(node, CommandClass.MultiChannel)
        {
        }

        public async Task BinarySwitchSet(byte endPointID, bool value)
        {
            var controllerID = await Node.Controller.GetNodeID();
            await Channel.Send(Node, new Command(Class, command.Encap, controllerID, endPointID, Convert.ToByte(CommandClass.SwitchBinary), Convert.ToByte(SwitchBinary.command.Set), value ? (byte)0xFF : (byte)0x00));
        }

        public async Task<byte[]> Get()
        {
            return await Channel.Send(Node, new Command(Class, command.EndPointGet), command.EndPointReport);
        }
    }
}
