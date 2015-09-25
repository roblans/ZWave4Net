using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Controller.CommandClasses
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
