using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave;
using ZWave.CommandClasses;

namespace ZWaveDriverSample
{
    public class Samples
    {
        public async Task TurnWallPlugOn()
        {
            // the nodeID of the wallplug
            byte wallPlugNodeID = 3;

            // create the controller
            var controller = new ZWaveController("COM1");
            
            // open the controller
            controller.Open();

            // get the included nodes
            var nodes = await controller.GetNodes();
            
            // get the wallplug
            var wallPlug = nodes[wallPlugNodeID];
            
            // get the SwitchBinary commandclass
            var switchBinary = wallPlug.GetCommandClass<SwitchBinary>();

            // turn wallplug on
            await switchBinary.Set(true);

            // close the controller
            controller.Close();
        }

        public async Task SensorAlarm()
        {
            // the nodeID of the motion sensor
            byte motionSensorID = 5;

            // create the controller
            var controller = new ZWaveController("COM1");

            // open the controller
            controller.Open();

            // get the included nodes
            var nodes = await controller.GetNodes();

            // get the motionSensor
            var motionSensor = nodes[motionSensorID];

            // get the SensorAlarm commandclass
            var sensorAlarm = motionSensor.GetCommandClass<SensorAlarm>();

            // subscribe to alarm event
            sensorAlarm.Changed += (s, e) => Console.WriteLine("Alarm");

            // wait
            Console.ReadLine();

            // close the controller
            controller.Close();
        }
    }
}
