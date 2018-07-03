using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZWave.CommandClasses;
using ZWave.Channel;
using System.Collections;
using System;
using System.Threading;

namespace ZWave
{
    public class Node
    {
        private static byte functionID = 0;
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
            _commandClasses.Add(new MultiChannelAssociation(this));
        }

        private static byte GetNextFunctionID()
        {
            lock (typeof(Node)) { return functionID = (byte)((functionID % 255) + 1); }
        }

        protected ZWaveChannel Channel
        {
            get { return Controller.Channel; }
        }

        public T GetCommandClass<T>()  where T : ICommandClass
        {
            return _commandClasses.OfType<T>().FirstOrDefault();
        }

        public Task<VersionCommandClassReport[]> GetSupportedCommandClasses()
        {
            return GetSupportedCommandClasses(CancellationToken.None);
        }

        public async Task<VersionCommandClassReport[]> GetSupportedCommandClasses(CancellationToken cancellationToken)
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
                var report = await version.GetCommandClass(commandClass, cancellationToken);
                commandClassVersions[commandClass] = report;
            }

            _commandClassVersions = commandClassVersions;
            lock(_commandClassVersions)
            {
                return _commandClassVersions.Values.Where(r => r.Version > 0).ToArray();
            }
        }

        public Task<NodeProtocolInfo> GetProtocolInfo()
        {
            return GetProtocolInfo(CancellationToken.None);
        }

        public async Task<NodeProtocolInfo> GetProtocolInfo(CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Function.GetNodeProtocolInfo, cancellationToken, NodeID);
            return NodeProtocolInfo.Parse(response);
        }

        public Task<NeighborUpdateStatus> RequestNeighborUpdate(Action<NeighborUpdateStatus> progress = null)
        {
            return RequestNeighborUpdate(progress, CancellationToken.None);
        }


        public async Task<NeighborUpdateStatus> RequestNeighborUpdate(Action<NeighborUpdateStatus> progress, CancellationToken cancellationToken)
        {
            // get next functionID (1..255) 
            var functionID = GetNextFunctionID();

            // send request, pass current node and functionID
            var response = await Channel.Send(Function.RequestNodeNeighborUpdate, new byte[] { NodeID, functionID }, (payload) =>
            {
                // check if repsonse matches request 
                if (payload[0] == functionID)
                {
                    // yes, so parse status
                    var status =(NeighborUpdateStatus)payload[1];

                    // if callback delegate provided then invoke with progress 
                    progress?.Invoke(status);

                    // return true when final state reached (we're done)
                    return status == NeighborUpdateStatus.Done || status == NeighborUpdateStatus.Failed;
                }
                return false;
            }, cancellationToken);

            // return the status of the final reponse
            return (NeighborUpdateStatus)response[1];
        }

        public Task<Node[]> GetNeighbours()
        {
            return GetNeighbours(CancellationToken.None);
        }

        public async Task<Node[]> GetNeighbours(CancellationToken cancellationToken)
        {
            var nodes = await Controller.GetNodes();
            var results = new List<Node>();

            var response = await Channel.Send(Function.GetRoutingTableLine, cancellationToken, NodeID);
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

        internal async Task<VersionCommandClassReport> GetCommandClassVersionReport(CommandClass commandClass, CancellationToken cancellationToken)
        {
            lock(_commandClassVersions)
            {
                if (_commandClassVersions.ContainsKey(commandClass))
                    return _commandClassVersions[commandClass];
            }

            // The version isn't cached, so we should bring it now.
            //
            var version = GetCommandClass<CommandClasses.Version>();
            var report = await version.GetCommandClass(commandClass, cancellationToken);
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
