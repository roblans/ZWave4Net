using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.CommandClasses
{
    public class NodeReport 
    {
        public readonly Node Node;

        public NodeReport(Node node)
        {
            if ((Node = node) == null)
                throw new ArgumentNullException(nameof(node));
        }
    }
}
