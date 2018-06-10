using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZWave.CommandClasses;
using ZWave.Channel;
using System.Collections;
using System;

namespace ZWave
{
    public class Node
    {
        private List<CommandClassBase> _commandClasses = new List<CommandClassBase>();
        private IDictionary<CommandClass, VersionCommandClassReport> _commandClassVersions = new Dictionary<CommandClass, VersionCommandClassReport>();

        public readonly byte NodeID;
        public readonly ZWaveController Controller;

        public event System.EventHandler<NodeEventArgs> UnknownCommandReceived;
        public event System.EventHandler<EventArgs> UpdateReceived;

        public Node(byte nodeID, ZWaveController contoller)
        {
            NodeID = nodeID;
            Controller = contoller;

            _commandClasses.Add(new Basic(this));
            _commandClasses.Add(new ManufacturerSpecific(this));
            _commandClasses.Add(new Battery(this));
            _commandClasses.Add(new SwitchMultiLevel(this));
            _commandClasses.Add(new Alarm(this));
            _commandClasses.Add(new Association(this));
            _commandClasses.Add(new SensorBinary(this));
            _commandClasses.Add(new SensorAlarm(this));
            _commandClasses.Add(new SensorMultiLevel(this));
            _commandClasses.Add(new WakeUp(this));
            _commandClasses.Add(new Meter(this));
            _commandClasses.Add(new SwitchBinary(this));
            _commandClasses.Add(new ZWave.CommandClasses.Version(this));
            _commandClasses.Add(new Configuration(this));
            _commandClasses.Add(new Color(this));
            _commandClasses.Add(new MultiChannel(this));
            _commandClasses.Add(new ThermostatSetpoint(this));
            _commandClasses.Add(new Clock(this));
            _commandClasses.Add(new CentralScene(this));
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
            var commandClassVersions = new Dictionary<CommandClass, VersionCommandClassReport>();
            foreach (var commandClass in System.Enum.GetValues(typeof(CommandClass)).Cast<CommandClass>())
            {
                var report = await version.GetCommandClass(commandClass);
                commandClassVersions[commandClass] = report;
            }

            _commandClassVersions = commandClassVersions;
            lock(_commandClassVersions)
            {
                return _commandClassVersions.Values.Where(r => r.Version > 0).ToArray();
            }
        }

        public async Task<NodeProtocolInfo> GetProtocolInfo()
        {
            var response = await Channel.Send(Function.GetNodeProtocolInfo, NodeID);
            return NodeProtocolInfo.Parse(response);
        }

        public async Task<RequestNeighborUpdateReport> RequestNeighborUpdate(Action<RequestNeighborUpdateReport> progress = null)
        {
            var response = await Channel.Send(Function.RequestNodeNeighborUpdate, new byte[] { NodeID }, (payload) =>
            {
                var report = new RequestNeighborUpdateReport(this, payload);
                progress?.Invoke(report);
                return report.IsCompleted;
            });
            return new RequestNeighborUpdateReport(this, response);
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

        internal async Task<VersionCommandClassReport> GetCommandClassVersionReport(CommandClass commandClass)
        {
            lock(_commandClassVersions)
            {
                if (_commandClassVersions.ContainsKey(commandClass))
                    return _commandClassVersions[commandClass];
            }

            // The version isn't cached, so we should bring it now.
            //
            var version = GetCommandClass<CommandClasses.Version>();
            var report = await version.GetCommandClass(commandClass);
            lock (_commandClassVersions)
            {
                _commandClassVersions[commandClass] = report;
            }

            return report;
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

        internal void HandleUpdate()
        {
            OnUpdateReceived(EventArgs.Empty);
        }
        
        protected virtual void OnUpdateReceived(EventArgs args)
        {
            UpdateReceived?.Invoke(this, args);
        }

        protected virtual void OnUnknownCommandReceived(NodeEventArgs args)
        {
            UnknownCommandReceived?.Invoke(this, args);
        }
    }
}
