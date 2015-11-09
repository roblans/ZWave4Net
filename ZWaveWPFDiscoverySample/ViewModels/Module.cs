using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave;

namespace ZWaveWPFDiscoverySample.ViewModels
{
    public class Module : ViewModel
    {

        
        public object[] Nodes { get; private set; }

        private readonly ZWaveController controller;

        public Module()
        {
            var portName = System.IO.Ports.SerialPort.GetPortNames().Where(element =>
                element != "COM1"
                && element != "COM10"
                && element != "COM11").First();

            controller = new ZWaveController(portName);
            controller.Open();

            LoadNodes();
        }


        public async void LoadNodes()
        {
            var nodes = await controller.GetNodes();
            Nodes = nodes.Select(nd => new Node(nd)).ToArray();

            Notify(nameof(nodes));
        }
    }
}
