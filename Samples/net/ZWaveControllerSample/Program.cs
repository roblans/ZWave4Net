using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave.Controller;
using ZWave.Controller.CommandClasses;
using ZWave.Channel;

namespace ZWaveDriverSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var seconds = PayloadConverter.GetBytes(7200);
            var interval = PayloadConverter.ToUInt32(seconds);

            var portName = System.IO.Ports.SerialPort.GetPortNames().First();

            var controller = new ZWaveContoller(portName);
            //driver.Channel.Log = Console.Out;

            controller.Open();
            try
            {
                Run(controller).Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var inner in ex.InnerExceptions)
                {
                    Console.WriteLine($"{inner.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            finally
            {
                Console.ReadLine();
                controller.Close();
            }
        }

        static private async Task Run(ZWaveContoller driver)
        {
            Console.WriteLine($"Version: {await driver.GetVersion()}");
            Console.WriteLine($"HomeID: {await driver.GetHomeID():X}");
            Console.WriteLine($"ControllerID: {await driver.GetContollerID():D3}");

            Console.WriteLine();
            var nodes = await driver.GetNodes();
            foreach (var node in nodes)
            {
                var protocolInfo = await node.GetNodeProtocolInfo();

                // dump node
                Console.WriteLine($"Node: {node}, Generic = {protocolInfo.GenericType}, Basic = {protocolInfo.BasicType}, Listening = {protocolInfo.IsListening} ");
            }

            // NodeID of the fibaro wall plug
            byte wallPlugID = 6;
            await RunWallplugTest(nodes[wallPlugID]);

            // NodeID of the fibaro motionsensor
            byte motionSensorID = 5;
            await RunMotionSensorTest(nodes[motionSensorID]);
        }

        private static async Task RunWallplugTest(Node wallPlug)
        {
            var basic = wallPlug.GetCommandClass<Basic>();
            basic.Changed += (_, e) => Console.WriteLine($"Basic report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var sensorMultiLevel = wallPlug.GetCommandClass<SensorMultiLevel>();
            sensorMultiLevel.Changed += (_, e) => Console.WriteLine($"SensorMultiLevel report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var meter = wallPlug.GetCommandClass<Meter>();
            meter.Changed += (_, e) => Console.WriteLine($"Meter report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var association = wallPlug.GetCommandClass<Association>();
            await association.Add(1, 1);
            await association.Add(2, 1);
            await association.Add(3, 1);

            var basicReport = await basic.Get();
            Console.WriteLine($"Basic report of Node {basicReport.Node:D3} is [{basicReport}]");

            //Console.WriteLine($"Toggle basicvalue of Node {basicReport.Node:D3}");
            //await basic.Set((byte)(basicReport.Value == 0x00 ? 0xFF : 0x00));

            basicReport = await basic.Get();
            Console.WriteLine($"Basic report of Node {basicReport.Node:D3} is [{basicReport}]");

            var manufacturerSpecific = wallPlug.GetCommandClass<ManufacturerSpecific>();
            var manufacturerSpecificReport = await manufacturerSpecific.Get();
            Console.WriteLine($"Manufacturer specific report of Node {manufacturerSpecificReport.Node:D3} is [{manufacturerSpecificReport}]");

            var sensorMultiLevelReport = await sensorMultiLevel.Get();
            Console.WriteLine($"SensorMultiLevel report of Node {sensorMultiLevelReport.Node:D3} is [{sensorMultiLevelReport}]");

            var meterReport = await meter.Get();
            Console.WriteLine($"MeterReport report of Node {meterReport.Node:D3} is [{meterReport}]");

            Console.ReadLine();
        }

        private static async Task RunMotionSensorTest(Node motionSensor)
        {
            var basic = motionSensor.GetCommandClass<Basic>();
            basic.Changed += (_, e) => Console.WriteLine($"Basic report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var alarm = motionSensor.GetCommandClass<Alarm>();
            alarm.Changed += (_, e) => Console.WriteLine($"Alarm report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var sensorBinary = motionSensor.GetCommandClass<SensorBinary>();
            sensorBinary.Changed += (_, e) => Console.WriteLine($"SensorBinary report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var sensorAlarm = motionSensor.GetCommandClass<SensorAlarm>();
            sensorAlarm.Changed += (_, e) => Console.WriteLine($"SensorAlarm report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var sensorMultiLevel = motionSensor.GetCommandClass<SensorMultiLevel>();
            sensorMultiLevel.Changed += (_, e) => Console.WriteLine($"SensorMultiLevel report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var wakeUp = motionSensor.GetCommandClass<WakeUp>();
            wakeUp.Notification += (_, e) => Console.WriteLine($"WakeUp report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            Console.WriteLine("Please wakeup the motion sensor.");
            Console.ReadLine();

            var association = motionSensor.GetCommandClass<Association>();
            await association.Add(1, 1);
            await association.Add(2, 1);
            await association.Add(3, 1);

            var manufacturerSpecific = motionSensor.GetCommandClass<ManufacturerSpecific>();
            var manufacturerSpecificReport = await manufacturerSpecific.Get();
            Console.WriteLine($"Manufacturer specific report of Node {manufacturerSpecificReport.Node:D3} is [{manufacturerSpecificReport}]");

            var battery = motionSensor.GetCommandClass<Battery>();
            var batteryReport = await battery.Get();
            Console.WriteLine($"Battery report of Node {batteryReport.Node:D3} is [{batteryReport}]");

            var sensorMultiLevelReport = await sensorMultiLevel.Get();
            Console.WriteLine($"SensorMultiLevel report of Node {sensorMultiLevelReport.Node:D3} is [{sensorMultiLevelReport}]");

            await wakeUp.SetInterval(TimeSpan.FromHours(2), 0x01);
            var wakeUpReport = await wakeUp.GetInterval();
            Console.WriteLine($"WakeUp report of Node {wakeUpReport.Node:D3} is [{wakeUpReport}]");

            Console.ReadLine();
        }
    }
}