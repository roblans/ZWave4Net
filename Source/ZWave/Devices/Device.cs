using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.CommandClasses;

namespace ZWave.Devices
{
    public class Device
    {
        public readonly Node Node;
        public string Name { get; set; }
        public Device(Node node)
        {
            Node = node;
        }

        public override string ToString()
        {
            return Name ?? Node.ToString();
        }
    }
}
