using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Driver.CommandClasses
{
    public class NodeReport 
    {
        public readonly Node Node;

        public NodeReport(Node node)
        {
            Node = node;
        }
    }
}
