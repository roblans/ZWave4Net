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
        private List<CommandClassBase> _commandClasses = new List<CommandClassBase>();
        private IDictionary<CommandClass, VersionCommandClassReport> _commandClassVersions = new Dictionary<CommandClass, VersionCommandClassReport>();

        public readonly byte NodeID;
        public readonly ZWaveController Controller;

        /// <summary>
        /// Will be fired when unknown command received.
        /// </summary>
        public event EventHandler<NodeEventArgs> UnknownCommandReceived;

        /// <summary>
        /// Will be fired when node update command received.
        /// </summary>
        public event EventHandler<EventArgs> UpdateReceived;

        /// <summary>
        /// Will be fired when any command received.
        /// </summary>
        public event EventHandler<EventArgs> MessageReceived;

        public Node(byte nodeID, ZWaveController contoller)
        {
            NodeID = nodeID;
            Controller = contoller;
            _commandClasses.Add(new Alarm(this));
            _commandClasses.Add(new Association(this));
            _commandClasses.Add(new Basic(this));
            _commandClasses.Add(new Battery(this));
            _commandClasses.Add(new CentralScene(this));
            _commandClasses.Add(new Clock(this));
            _commandClasses.Add(new Color(this));
            _commandClasses.Add(new Configuration(this));
            _commandClasses.Add(new ManufacturerSpecific(this));
            _commandClasses.Add(new Meter(this));
            _commandClasses.Add(new MultiChannel(this));
            _commandClasses.Add(new MultiChannelAssociation(this));
            _commandClasses.Add(new NodeNaming(this));
            _commandClasses.Add(new Notification(this));
            _commandClasses.Add(new SceneActivation(this));
            _commandClasses.Add(new Schedule(this));
            _commandClasses.Add(new CommandClasses.Security(this));
            _commandClasses.Add(new SensorAlarm(this));
            _commandClasses.Add(new SensorBinary(this));
            _commandClasses.Add(new SensorMultiLevel(this));
            _commandClasses.Add(new SwitchBinary(this));
            _commandClasses.Add(new SwitchMultiLevel(this));
            _commandClasses.Add(new SwitchToggleBinary(this));
            _commandClasses.Add(new SwitchToggleMultiLevel(this));
            _commandClasses.Add(new ThermostatFanMode(this));
            _commandClasses.Add(new ThermostatFanState(this));
            _commandClasses.Add(new ThermostatMode(this));
            _commandClasses.Add(new ThermostatOperatingState(this));
            _commandClasses.Add(new ThermostatSetpoint(this));
            _commandClasses.Add(new ZWave.CommandClasses.Version(this));
            _commandClasses.Add(new WakeUp(this));
        }

        protected ZWaveChannel Channel
        {
            get { return Controller.Channel; }
        }

        public T GetCommandClass<T>()  where T : ICommandClass
        {
            return _commandClasses.OfType<T>().FirstOrDefault();
        }

        public async Task<VersionCommandClassReport[]> GetSupportedCommandClasses(CancellationToken cancellationToken = default)
        {
            // is this node the controller?
            if (await Controller.GetNodeID() == NodeID)
            {
                // yes, so return an empty collection. GetSupportedCommandClasses is not supported by the controller
                return new VersionCommandClassReport[0];
            }

            var version = GetCommandClass<CommandClasses.Version>();
            var commandClassVersions = new Dictionary<CommandClass, VersionCommandClassReport>();
            foreach (var commandClass in Enum.GetValues(typeof(CommandClass)).Cast<CommandClass>())
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

        public async Task<NeighborUpdateStatus> RequestNeighborUpdate(Action<NeighborUpdateStatus> progress = null, CancellationToken cancellationToken = default)
        {
            // send request, pass current node and functionID. In some cases, if the controller is busy, he won't send the START status.
            // In this case, we will wait some time and retry.
            bool operationStarted = false;
            byte[] response = null;
            for (int i = 0; i < 8 && !operationStarted; i++)
            {
                // get next functionID (1..255) 
                response = await Channel.SendWithFunctionId(Function.RequestNodeNeighborUpdate, new byte[] { NodeID }, (payload) =>
                {
                    // Parse status
                    var status = (NeighborUpdateStatus)payload[0];
                    if (!operationStarted && status != NeighborUpdateStatus.Started)
                    {
                        return true;
                    }

                    operationStarted = true;

                    // if callback delegate provided then invoke with progress 
                    progress?.Invoke(status);

                    // return true when final state reached (we're done)
                    return status == NeighborUpdateStatus.Done || status == NeighborUpdateStatus.Failed;
                }, cancellationToken);

                if (!operationStarted)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }

            // return the status of the final response
            return (NeighborUpdateStatus)response[1];
        }

        public async Task<Node[]> GetNeighbours(CancellationToken cancellationToken = default)
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

        public async Task<bool> IsNodeFailed(CancellationToken cancellationToken = default)
        {
            var response = await Channel.Send(Function.IsFailedNode, cancellationToken, NodeID);
            return response.Length > 0 && response[0] > 0;
        }

        public async Task RemoveFailedNode(CancellationToken cancellationToken = default)
        {
            var response = await Channel.Send(Function.RemoveFailedNodeId, cancellationToken, NodeID);
            if (response.Length == 0 || response[0] != 0)
            {
                throw new InvalidOperationException("Remove failed node operation failed. Make sure that the node is in failed state first.");
            }
        }

        /// <summary>
        /// Attempt to heal the node network.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The operation status.</returns>
        public async Task<HealNetworkStatus> HealNodeNetwork(CancellationToken cancellationToken = default)
        {
            const int maximumNumberOfReturnRoute = 5;
            NeighborUpdateStatus neighborUpdateStatus = await RequestNeighborUpdate(cancellationToken: cancellationToken);
            if (neighborUpdateStatus != NeighborUpdateStatus.Done)
            {
                return HealNetworkStatus.Failed;
            }

            Association association = GetCommandClass<Association>();
            AssociationGroupsReport associationGroupsReport = await association.GetGroups(cancellationToken);
            List<byte> asociatedNodesIds = new List<byte>(maximumNumberOfReturnRoute);
            for (byte i = 1; i <= associationGroupsReport.GroupsSupported && asociatedNodesIds.Count < maximumNumberOfReturnRoute; i++)
            {
                AssociationReport associationReport = await association.Get(i, cancellationToken);
                foreach(byte nodeId in associationReport.Nodes)
                {
                    if (!asociatedNodesIds.Contains(nodeId))
                    {
                        asociatedNodesIds.Add(nodeId);
                        if (asociatedNodesIds.Count == maximumNumberOfReturnRoute)
                        {
                            break;
                        }
                    }
                }
            }

            await DeleteAllReturnRoute(cancellationToken);
            foreach (byte associatedNodeId in asociatedNodesIds)
            {
                await AssignReturnRoute(associatedNodeId, cancellationToken);
            }

            return HealNetworkStatus.Succeeded;
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
            MessageReceived?.Invoke(this, EventArgs.Empty);
            var target = _commandClasses.FirstOrDefault(element => Convert.ToByte(element.Class) == command.ClassID);
            if (target != null)
            {
                target.HandleEvent(command);
            }
            else
            {
                OnUnknownCommandReceived(new NodeEventArgs(NodeID, command));
            }
        }

        internal void HandleUpdate()
        {
            MessageReceived?.Invoke(this, EventArgs.Empty);
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

        private async Task DeleteAllReturnRoute(CancellationToken cancellationToken)
        {
            await Channel.SendWithFunctionId(Function.DeleteReturnRoute, new byte[] { NodeID }, null, cancellationToken);
        }

        private async Task AssignReturnRoute(byte associatedNodeId, CancellationToken cancellationToken)
        {
            await Channel.SendWithFunctionId(Function.AssignReturnRoute, new byte[] { NodeID, associatedNodeId }, null, cancellationToken);
        }
    }
}
