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

        public Device(Node node)
        {
            Node = node;
        }
    }
}
