using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZWave.CommandClasses;
using ZWave.Channel;
using System.Collections;

namespace ZWave
{
    public class Node
    {
        private List<CommandClassBase> _commandClasses = new List<CommandClassBase>();

        public readonly byte NodeID;
        public readonly ZWaveController Controller;

        public event System.EventHandler<NodeEventArgs> UnknownCommandReceived;

        public Node(byte nodeID, ZWaveController contoller)
        {
            NodeID = nodeID;
            Controller = contoller;

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
            _commandClasses.Add(new SwitchBinary(this));
            _commandClasses.Add(new Version(this));
            _commandClasses.Add(new Configuration(this));
            _commandClasses.Add(new Color(this));
            _commandClasses.Add(new MultiChannel(this));
            _commandClasses.Add(new ThermostatSetpoint(this));
            _commandClasses.Add(new Clock(this));
            _commandClasses.Add(new SceneActivation(this));
        }

        protected ZWaveChannel Channel
        {
            get { return Controller.Channel; }
        }

        public T GetCommandClass<T>()  where T : ICommandClass
        {
            return _commandClasses.OfType<T>().FirstOrDefault();
        }

        public async Task<VersionCommandClassReport[]> GetSupportedCommandClasses()
        {
            // is this node the controller?
            if (await Controller.GetNodeID() == NodeID)
            {
                // yes, so return an empty collection. GetSupportedCommandClasses is not supported by the controller
                return new VersionCommandClassReport[0];
            }

            var version = GetCommandClass<CommandClasses.Version>();
            var reports = new List<VersionCommandClassReport>();
            foreach(var commandClass in System.Enum.GetValues(typeof(CommandClass)).Cast<CommandClass>())
            {
                var report = await version.GetCommandClass(commandClass);
                if (report.Version == 0)
                    continue;

                reports.Add(report);
            }
            return reports.ToArray();
        }

        public async Task<NodeProtocolInfo> GetProtocolInfo()
        {
            var response = await Channel.Send(Function.GetNodeProtocolInfo, NodeID);
            return NodeProtocolInfo.Parse(response);
        }

        public async Task<Node[]> GetNeighbours()
        {
            var nodes = await Controller.GetNodes();
            var results = new List<Node>();

            var response = await Channel.Send(Function.GetRoutingTableLine, NodeID);
            var bits = new BitArray(response);
            for (byte i = 0; i < bits.Length; i++)
            {
                if (bits[i])
                {
                    results.Add(nodes[(byte)(i + 1)]);
                }
            }
            return results.ToArray();
        }

        public override string ToString()
        {
            return $"{NodeID:D3}";
        }

        internal void HandleEvent(Command command)
        {
            var target = _commandClasses.FirstOrDefault(element => System.Convert.ToByte(element.Class) == command.ClassID);
            if (target != null)
            {
                target.HandleEvent(command);
            }
            else
            {
                OnUnknownCommandReceived(new ZWave.Channel.NodeEventArgs(NodeID, command));
            }
        }

        public virtual void OnUnknownCommandReceived(NodeEventArgs args)
        {
            UnknownCommandReceived?.Invoke(this, args);
        }
    }
}
