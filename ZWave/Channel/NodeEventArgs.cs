using System;

namespace ZWave.Channel
{
    public class NodeEventArgs : EventArgs
    {
        public static new readonly NodeEventArgs Empty = new NodeEventArgs(0, new Command(CommandClass.NoOperation, 0, new byte[0]));

        public readonly byte NodeID;
        public readonly Command Command;

        public NodeEventArgs(byte nodeID, Command command)
        {
            if ((NodeID = nodeID) == 0)
                throw new ArgumentOutOfRangeException(nameof(NodeID), nodeID, "NodeID can not be 0");
            if ((Command = command) == null)
                throw new ArgumentNullException(nameof(command));
        }
    }
}
