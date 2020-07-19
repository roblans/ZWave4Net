using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;
using ZWave.Channel.Protocol;

namespace ZWave
{
    public class ZWaveController
    {
        private Task<NodeCollection> _getNodes;
        private string _version;
        private uint? _homeID;
        private byte? _nodeID;
        public readonly ZWaveChannel Channel;
        public event EventHandler<ErrorEventArgs> Error;
        public event EventHandler ChannelClosed;
        public event EventHandler<NodesNetworkChangeEventArgs> NodesNetworkChanged;

        private ZWaveController(ZWaveChannel channel)
        {
            Channel = channel;
        }

        public ZWaveController(ISerialPort port)
            : this(new ZWaveChannel(port))
        {
        }

#if NET || WINDOWS_UWP || NETCOREAPP2_0 || NETCOREAPP3_0 || NETSTANDARD2_0
        public ZWaveController(string portName)
            : this(new ZWaveChannel(portName))
        {
        }
#endif

#if WINDOWS_UWP
        public ZWaveController(ushort vendorId, ushort productId)
             : this(new ZWaveChannel(vendorId, productId))
        {
        }
#endif

        protected virtual void OnError(ErrorEventArgs e)
        {
            Error?.Invoke(this, e);
        }

        protected virtual void OnChannelClosed(EventArgs e)
        {
            ChannelClosed?.Invoke(this, e);
        }

        public void Open()
        {
            Channel.NodeEventReceived += Channel_NodeEventReceived;
            Channel.NodeUpdateReceived += Channel_NodeUpdateReceived;
            Channel.NodesNetworkChangeOccurred += Channel_NodesNetworkChangeOccurred;
            Channel.Error += Channel_Error;
            Channel.Closed += Channel_Closed;
            Channel.Open();
        }

        private enum AddRemoveNodeStatus
        {
            NodeStatusLearnReady = 1,
            NodeStatusNodeFound = 2,
            NodeStatusAddingSlave = 3,
            NodeStatusAddingController = 4,
            AddNodeSatusProtocolDone = 5,
            NodeSatusDone = 6,
            NodeSatusFailed = 7
        }

        private async void Channel_NodesNetworkChangeOccurred(object sender, ControllerFunctionMessage e)
        {
            var nodes = await GetNodes();
            AddRemoveNodeStatus operationStatus = (AddRemoveNodeStatus)e.Payload[1];
            byte nodeId = e.Payload[2];
            if (operationStatus == AddRemoveNodeStatus.NodeStatusAddingSlave && nodeId > 0)
            {
                bool isAddNode = e.Function == Function.AddNodeToNetwork;
                if (isAddNode)
                {
                    if (nodes[nodeId] != null)
                    {
                        // This is attempt to add the same node twice.
                        return;
                    }

                    nodes.Add(new Node(nodeId, this));
                }
                else
                {
                    nodes.RemoveById(nodeId);
                }

                byte dataLength = e.Payload[3];
                if (dataLength <= 3)
                {
                    throw new FormatException("Expected to have node information on node status adding slave response.");
                }

                CommandClass[] commandClasses = e.Payload.Skip(7).Select(b => (CommandClass)b).ToArray();
                NodesNetworkChanged?.Invoke(this, new NodesNetworkChangeEventArgs(isAddNode, nodeId, commandClasses));
            }
        }

        private void Channel_Error(object sender, ErrorEventArgs e)
        {
            OnError(e);
        }

        private void Channel_Closed(object sender, EventArgs e)
        {
            OnChannelClosed(e);
        }

        private async void Channel_NodeEventReceived(object sender, NodeEventArgs e)
        {
            try
            {
                var nodes = await GetNodes();
                var target = nodes[e.NodeID];
                if (target != null)
                {
                    target.HandleEvent(e.Command);
                }
            }
            catch(Exception ex)
            {
                OnError(new ErrorEventArgs(ex));
            }
        }

        private async void Channel_NodeUpdateReceived(object sender, NodeUpdateEventArgs e)
        {
            try
            {
                var nodes = await GetNodes();
                var target = nodes[e.NodeID];
                if (target != null)
                {
                    target.HandleUpdate();
                }
            }
            catch (Exception ex)
            {
                OnError(new ErrorEventArgs(ex));
            }
        }

        public void Close()
        {
            Channel.NodesNetworkChangeOccurred -= Channel_NodesNetworkChangeOccurred;
            Channel.Error -= Channel_Error;
            Channel.NodeEventReceived -= Channel_NodeEventReceived;
            Channel.NodeUpdateReceived -= Channel_NodeUpdateReceived;
            Channel.Close();
        }

        public async Task<string> GetVersion(CancellationToken cancellationToken = default)
        {
            if (_version == null)
            {
                var response = await Channel.Send(Function.GetVersion, cancellationToken);
                var data = response.TakeWhile(element => element != 0).ToArray();
                _version = Encoding.UTF8.GetString(data, 0, data.Length);
            }
            return _version;
        }

        public async Task<uint> GetHomeID(CancellationToken cancellationToken = default)
        {
            if (_homeID == null)
            {
                var response = await Channel.Send(Function.MemoryGetId, cancellationToken);
                _homeID = PayloadConverter.ToUInt32(response);
            }
            return _homeID.Value;
        }

        public async Task<byte> GetNodeID(CancellationToken cancellationToken = default)
        {
            if (_nodeID == null)
            {
                var response = await Channel.Send(Function.MemoryGetId, cancellationToken);
                _nodeID = response[4];
            }
            return _nodeID.Value;
        }

        public Task<NodeCollection> DiscoverNodes(CancellationToken cancellationToken = default)
        {
            return _getNodes = Task.Run(async () =>
            {
                var response = await Channel.Send(Function.DiscoveryNodes, cancellationToken);
                var values = response.Skip(3).Take(29).ToArray();

                var nodes = new NodeCollection();
                var bits = new BitArray(values);
                for (byte i = 0; i < bits.Length; i++)
                {
                    if (bits[i])
                    {
                        var node = new Node((byte)(i + 1), this);
                        nodes.Add(node);
                    }
                }
                return nodes;
            });
        }

        public async Task<NodeCollection> GetNodes(CancellationToken cancellationToken = default)
        {
            return await (_getNodes ?? (_getNodes = DiscoverNodes(cancellationToken)));
        }

        [Flags]
        private enum AddRemoveNodeMode : byte
        {
            NodeAny = 0x01,
            NodeStop = 0x05,
            NodeOptionNetworkWide = 0x40,
            NodeOptionNormalPower = 0x80
        }

        /// <summary>
        /// Start adding nodes to the network.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if operation succeeded. False othersise.</returns>
        public async Task<bool> StartAddingNodesToNetwork(CancellationToken cancellationToken = default)
        {
            byte[] res = await Channel.SendWithFunctionId(Function.AddNodeToNetwork, new byte[] { (byte)(AddRemoveNodeMode.NodeAny | AddRemoveNodeMode.NodeOptionNetworkWide | AddRemoveNodeMode.NodeOptionNormalPower) }, null, cancellationToken);
            return (AddRemoveNodeStatus)res[1] == AddRemoveNodeStatus.NodeStatusLearnReady;
        }

        /// <summary>
        /// Stop adding nodes to the network.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if operation succeeded. False othersise.</returns>
        public async Task<bool> StopAddingNodesToNetwork(CancellationToken cancellationToken = default)
        {
            byte[] res = await Channel.SendWithFunctionId(Function.AddNodeToNetwork, new byte[] { (byte)AddRemoveNodeMode.NodeStop }, null, cancellationToken);
            return (AddRemoveNodeStatus)res[1] == AddRemoveNodeStatus.NodeSatusDone;
        }

        /// <summary>
        /// Start removing node from the network.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if operation succeeded. False othersise.</returns>
        public async Task<bool> StartRemoveNodeFromNetwork(CancellationToken cancellationToken = default)
        {
            byte[] res = await Channel.SendWithFunctionId(Function.RemoveNodeFromNetwork, new byte[] { (byte)(AddRemoveNodeMode.NodeAny | AddRemoveNodeMode.NodeOptionNetworkWide) }, null, cancellationToken);
            return (AddRemoveNodeStatus)res[1] == AddRemoveNodeStatus.NodeStatusLearnReady;
        }

        /// <summary>
        /// Stop removing node from the network.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if operation succeeded. False othersise.</returns>
        public async Task<bool> StopRemoveNodeFromNetwork(CancellationToken cancellationToken = default)
        {
            try
            {
                byte[] res = await Channel.SendWithFunctionId(Function.RemoveNodeFromNetwork, new byte[] { (byte)AddRemoveNodeMode.NodeStop }, null, cancellationToken);
                return (AddRemoveNodeStatus)res[1] == AddRemoveNodeStatus.NodeSatusDone;
            }
            catch (TimeoutException)
            {
                // In case the controller isn't in exclusion state, it throws timeout.
                // If we reach here we assume this is the case.
                return true;
            }
        }
    }
}
