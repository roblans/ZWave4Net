using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave4Net.Commands;

namespace ZWave4Net
{
    public class Node
    {
        private readonly List<CommandClass> _commandClasses = new List<CommandClass>();

        public readonly byte NodeID;
        public readonly ZWaveDriver Driver;

        public Node(byte nodeID, ZWaveDriver driver)
        {
            NodeID = nodeID;
            Driver = driver;

            //_commandClasses.Add(new Basic(this));
            //_commandClasses.Add(new Alarm(this));
            //_commandClasses.Add(new WakeUp(this));
            //_commandClasses.Add(new Battery(this));
            //_commandClasses.Add(new ManufacturerSpecific(this));
        }

        public override string ToString()
        {
            return NodeID.ToString("D3");
        }

        public T GetCommandClass<T>() where T : CommandClass
        {
            return _commandClasses.OfType<T>().First();
        }
    }
}
