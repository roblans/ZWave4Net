using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZWave.Driver.CommandClasses;
using ZWave.Driver.Communication;

namespace ZWave.Driver
{
    public class Node
    {
        private List<CommandClassBase> _commandClasses = new List<CommandClassBase>();

        public readonly byte NodeID;
        public readonly ZWaveChannel Channel;

        public Node(byte nodeID, ZWaveChannel channel)
        {
            NodeID = nodeID;
            Channel = channel;

            _commandClasses.Add(new Basic(this));
            _commandClasses.Add(new ManufacturerSpecific(this));
            _commandClasses.Add(new Battery(this));
            _commandClasses.Add(new Alarm(this));
            _commandClasses.Add(new Association(this));
            _commandClasses.Add(new SensorBinary(this));
            _commandClasses.Add(new SensorAlarm(this));
            _commandClasses.Add(new SensorMultiLevel(this));
            _commandClasses.Add(new WakeUp(this));
            _commandClasses.Add(new Meter(this));
        }

        public T GetCommandClass<T>()  where T : ICommandClass
        {
            return _commandClasses.OfType<T>().FirstOrDefault();
        }

        public async Task<NodeProtocolInfo> GetNodeProtocolInfo()
        {
            var response = await Channel.Send(Function.GetNodeProtocolInfo, NodeID);
            return NodeProtocolInfo.Parse(response);
        }

        public override string ToString()
        {
            return $"{NodeID:D3}";
        }

        internal void HandleEvent(Command command)
        {
            var target = _commandClasses.FirstOrDefault(element => Convert.ToByte(element.Class) == command.ClassID);
            if (target != null)
            {
                target.HandleEvent(command);
            }
        }
    }
}
