using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Communication
{
    public class NodeEventArgs : EventArgs
    {
        public readonly byte NodeID;
        public readonly Command Command;

        public NodeEventArgs(byte nodeID, Command command)
        {
            NodeID = nodeID;
            Command = command;
        }
    }
}
