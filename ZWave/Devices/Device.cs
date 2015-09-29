using System;
using System.Collections.Generic;
using System.Text;

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
