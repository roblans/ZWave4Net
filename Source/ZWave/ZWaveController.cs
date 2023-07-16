using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;
using ZWave.Channel.Protocol;
using ZWave.CommandClasses;

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

        public ZWaveController(string portName)
            : this(new ZWaveChannel(portName))
        {
        }

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

        public Task SoftReset(CancellationToken cancellationToken = default)
        {
            return Channel.SoftReset(cancellationToken);
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

        public async Task<ManufacturerSpecificReport> GetManufacturerSpecific(CancellationToken cancellationToken = default)
        {
            var response = await Channel.Send(Function.SerialGetCapabilities, cancellationToken);
            return new ManufacturerSpecificReport(new Node(await GetNodeID(), this), response.Skip(2).ToArray());
        }

        public async Task<Function[]> GetSupportedFunctions(CancellationToken cancellationToken = default)
        {
            var response = await Channel.Send(Function.SerialGetCapabilities, cancellationToken);
            var bits = new BitArray(response.Skip(8).ToArray());
            List<Function> functions = new List<Function>();
            for (short i = 0; i < bits.Length; i++)
            {
                if (bits[i])
                {
                    functions.Add((Function)i+1);
                }
            }
            return functions.ToArray();
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

        public async Task<sbyte[]> GetRSSIs(CancellationToken cancellationToken = default)
        {
            var response = await Channel.Send(Function.GetBackgroundRSSI, cancellationToken);
            List<sbyte> rssis = new List<sbyte>();
            foreach (byte b in response)
            {
                if (b != 0x7F)
                    rssis.Add((sbyte)b);
            }
            return rssis.ToArray();
        }

        public async Task<byte[]> GetLongRangeNodes(CancellationToken cancellationToken = default)
        {
            var response = await Channel.Send(Function.GetLRNodes, cancellationToken, new byte[] {0x0});
            var bits = new BitArray(response.Skip(3).ToArray());
            List<byte> nodeIds = new List<byte>();
            for (byte i = 0; i < 128; i++)
            {
                if (bits[i])
                    nodeIds.Add((byte)(i+1));
            }
            return nodeIds.ToArray();
        }

        public async Task<byte[]> BackupNVM(CancellationToken cancellationToken = default)
        {
            var open = await Channel.Send(Function.NVMBackupRestore, cancellationToken, new byte[] { 0x0, 0x0, 0x0, 0x0 });
            if (open[0] != 0)
                throw new InvalidOperationException($"Failed to open NVM.  Response {open[0]}");
            ushort len = PayloadConverter.ToUInt16(open, 2);
            byte[] buffer = new byte[len];
            try
            {
                ushort i = 0;
                while (i < len)
                {
                    var offset = PayloadConverter.GetBytes(i);
                    byte readLen = (byte)Math.Min(len - i, 255);
                    var read = await Channel.Send(Function.NVMBackupRestore, cancellationToken, new byte[] { 0x1, readLen, offset[0], offset[1] });
                    if (read[0] != 0 && read[0] != 0xFF)
                        throw new InvalidOperationException($"Failed to open NVM.  Response {open[0]}");
                    Buffer.BlockCopy(read, 4, buffer, i, read[1]);
                    i += read[1];
                }
            }
            finally
            {
                var close = await Channel.Send(Function.NVMBackupRestore, cancellationToken, new byte[] { 0x3, 0x0, 0x0, 0x0 });
                if (close[0] != 0)
                    throw new InvalidOperationException($"Backup Failed. Error {close[0]}");
            }
            return buffer;
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
