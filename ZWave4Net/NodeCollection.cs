using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net
{
    public class NodeCollection : IEnumerable<Node>
    {
        private readonly List<Node> _list = new List<Node>();
        
        internal void Add(Node node)
        {
            _list.Add(node);
        }

        public Node this[byte ID]
        {
            get { return _list.FirstOrDefault(element => element.NodeID == ID); }
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
