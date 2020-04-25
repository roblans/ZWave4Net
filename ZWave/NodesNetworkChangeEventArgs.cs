using System;
using ZWave.Channel;

namespace ZWave
{
    public class NodesNetworkChangeEventArgs : EventArgs
    {
        public bool IsNodeAdded { get; }

        public byte NodeId { get; }

        public CommandClass[] CommandClasses { get; }

        public NodesNetworkChangeEventArgs(bool isNodeAdded, byte nodeId, CommandClass[] commandClasses)
        {
            IsNodeAdded = isNodeAdded;
            NodeId = nodeId;
            CommandClasses = commandClasses;
        }
    }
}
