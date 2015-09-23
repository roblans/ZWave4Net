using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave.Driver;
using ZWave.Driver.CommandClasses;

namespace ZWaveDriverSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var portName = System.IO.Ports.SerialPort.GetPortNames().First();

            var driver = new ZWaveDriver(portName);
            driver.Channel.Log = Console.Out;

            driver.Open();
            try
            {
                Run(driver).Wait();
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
                driver.Close();
            }
        }

        static private async Task Run(ZWaveDriver driver)
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

            var basicReport = await basic.Get();
            Console.WriteLine($"Basic report of Node {basicReport.Node:D3} is [{basicReport}]");

            Console.WriteLine($"Toggle basicvalue of Node {basicReport.Node:D3}");
            await basic.Set((byte)(basicReport.Value == 0x00 ? 0xFF : 0x00));

            basicReport = await basic.Get();
            Console.WriteLine($"Basic report of Node {basicReport.Node:D3} is [{basicReport}]");

            var manufacturerSpecific = wallPlug.GetCommandClass<ManufacturerSpecific>();
            var manufacturerSpecificReport = await manufacturerSpecific.Get();

            Console.WriteLine($"Manufacturer specific report of Node {manufacturerSpecificReport.Node:D3} is [{manufacturerSpecificReport}]");
            Console.ReadLine();
        }

        private static async Task RunMotionSensorTest(Node motionSensor)
        {
            var basic = motionSensor.GetCommandClass<Basic>();
            basic.Changed += (_, e) => Console.WriteLine($"Basic report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var alarm = motionSensor.GetCommandClass<Alarm>();
            alarm.Changed += (_, e) => Console.WriteLine($"Alarm report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            Console.WriteLine("Please wakeup the motion sensor.");
            Console.ReadLine();


            var manufacturerSpecific = motionSensor.GetCommandClass<ManufacturerSpecific>();
            var manufacturerSpecificReport = await manufacturerSpecific.Get();
            Console.WriteLine($"Manufacturer specific report of Node {manufacturerSpecificReport.Node:D3} is [{manufacturerSpecificReport}]");

            var battery = motionSensor.GetCommandClass<Battery>();
            var batteryReport = await battery.Get();
            Console.WriteLine($"Battery report of Node {batteryReport.Node:D3} is [{batteryReport}]");

            var alarmReport = await alarm.Get();
            Console.WriteLine($"Alarm report of Node {alarmReport.Node:D3} is [{alarmReport}]");

            Console.ReadLine();
        }
    }
}