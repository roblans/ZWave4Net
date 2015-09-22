using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Driver
{
    public class Node
    {
        public readonly byte NodeID;
         
        public Node(byte nodeID)
        {
            NodeID = nodeID;
        }
    }
}
