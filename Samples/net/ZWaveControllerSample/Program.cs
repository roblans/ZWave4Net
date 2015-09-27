using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave.Controller;
using ZWave.Controller.CommandClasses;
using ZWave.Channel;
using System.IO;

namespace ZWaveDriverSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var portName = System.IO.Ports.SerialPort.GetPortNames().First();

            var controller = new ZWaveController(portName);
            //controller.Channel.Log = Console.Out;

            controller.Open();
            try
            {
                Run(controller).Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var inner in ex.InnerExceptions)
                {
                    LogMessage($"{inner.Message}");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"{ex.Message}");
            }
            finally
            {
                Console.ReadLine();
                controller.Close();
            }
        }

        private static void LogMessage(string message)
        {
            var text = $"{DateTime.Now.TimeOfDay} {message}";

            Console.WriteLine(text);
            lock(typeof(File))
            {
                if (Directory.Exists(@"D:\Temp"))
                {
                    File.AppendAllText(@"D:\Temp\ZWave.log", text + Environment.NewLine);
                }
            }
        }

        static private async Task Run(ZWaveController controller)
        {
            LogMessage($"Version: {await controller.GetVersion()}");
            LogMessage($"HomeID: {await controller.GetHomeID():X}");

            var controllerID = await controller.GetContollerID();
            LogMessage($"ControllerID: {controllerID:D3}");

            var nodes = await controller.GetNodes();
            foreach (var node in nodes)
            {
                var protocolInfo = await node.GetNodeProtocolInfo();

                // dump node
                LogMessage($"Node: {node}, Generic = {protocolInfo.GenericType}, Basic = {protocolInfo.BasicType}, Listening = {protocolInfo.IsListening} ");

                // subcribe to changes
                Subscribe(node);
            }

            // NodeID of the fibaro wall plug
            byte wallPlugID = 6;
            await RunWallplugTest(nodes[wallPlugID]);

            // NodeID of the fibaro motionsensor
            byte motionSensorID = 5;
            await RunMotionSensorTest(nodes[motionSensorID]);
        }

        private static void Subscribe(Node node)
        {
            var basic = node.GetCommandClass<Basic>();
            basic.Changed += (_, e) => LogMessage($"Basic report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var sensorMultiLevel = node.GetCommandClass<SensorMultiLevel>();
            sensorMultiLevel.Changed += (_, e) => LogMessage($"SensorMultiLevel report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var meter = node.GetCommandClass<Meter>();
            meter.Changed += (_, e) => LogMessage($"Meter report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var alarm = node.GetCommandClass<Alarm>();
            alarm.Changed += (_, e) => LogMessage($"Alarm report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var sensorBinary = node.GetCommandClass<SensorBinary>();
            sensorBinary.Changed += (_, e) => LogMessage($"SensorBinary report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var sensorAlarm = node.GetCommandClass<SensorAlarm>();
            sensorAlarm.Changed += (_, e) => LogMessage($"SensorAlarm report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var wakeUp = node.GetCommandClass<WakeUp>();
            wakeUp.Changed += (_, e) => LogMessage($"WakeUp report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var switchBinary = node.GetCommandClass<SwitchBinary>();
            switchBinary.Changed += (_, e) => LogMessage($"SwitchBinary report of Node {e.Report.Node:D3} changed to [{e.Report}]");
        }

        private static async Task RunWallplugTest(Node wallPlug)
        {
            var association = wallPlug.GetCommandClass<Association>();

            // associate group 1 - group 3 to controller node 
            await association.Add(1, 1);
            await association.Add(2, 1);
            await association.Add(3, 1);

            var supportedCommandClasses = await wallPlug.GetSupportedCommandClasses();
            LogMessage($"Supported commandclasses:\n{string.Join("\n", supportedCommandClasses.Cast<object>())}");

            var basic = wallPlug.GetCommandClass<Basic>();
            var basicReport = await basic.Get();
            LogMessage($"Basic report of Node {basicReport.Node:D3} is [{basicReport}]");

            var version = wallPlug.GetCommandClass<ZWave.Controller.CommandClasses.Version>();
            var versionReport = await version.Get();
            LogMessage($"VersionReport report of Node {versionReport.Node:D3} is [{versionReport}]");

            var commandClassVersionReport = await version.GetCommandClass(CommandClass.Meter);
            LogMessage($"CommandClassVersionReport report of Node {commandClassVersionReport.Node:D3} is [{commandClassVersionReport}]");

            var manufacturerSpecific = wallPlug.GetCommandClass<ManufacturerSpecific>();
            var manufacturerSpecificReport = await manufacturerSpecific.Get();
            LogMessage($"Manufacturer specific report of Node {manufacturerSpecificReport.Node:D3} is [{manufacturerSpecificReport}]");

            var sensorMultiLevel = wallPlug.GetCommandClass<SensorMultiLevel>();
            var sensorMultiLevelReport = await sensorMultiLevel.Get();
            LogMessage($"SensorMultiLevel report of Node {sensorMultiLevelReport.Node:D3} is [{sensorMultiLevelReport}]");

            var meter = wallPlug.GetCommandClass<Meter>();
            var meterSupportedReport = await meter.GetSupported();
            LogMessage($"MeterSupportedReport report of Node {meterSupportedReport.Node:D3} is [{meterSupportedReport}]");
            var meterReport = await meter.Get();
            LogMessage($"MeterReport report of Node {meterReport.Node:D3} is [{meterReport}]");

            var configuration = wallPlug.GetCommandClass<Configuration>();
            var configurationReport = await configuration.Get(47);
            LogMessage($"ConfigurationReport report of Node {configurationReport.Node:D3} is [{configurationReport}]");
                
            var switchBinary = wallPlug.GetCommandClass<SwitchBinary>();
            var switchBinaryReport = await switchBinary.Get();
            LogMessage($"SwitchBinaryReport report of Node {switchBinaryReport.Node:D3} is [{switchBinaryReport}]");

            await switchBinary.Set((byte)(switchBinaryReport.Value == 0x00 ? 0xFF : 0x00));

            Console.ReadLine();
        }

        private static async Task RunMotionSensorTest(Node motionSensor)
        {
            LogMessage("Please wakeup the motion sensor.");
            Console.ReadLine();

            var association = motionSensor.GetCommandClass<Association>();
            await association.Add(1, 1);
            await association.Add(2, 1);
            await association.Add(3, 1);

            var supportedCommandClasses = await motionSensor.GetSupportedCommandClasses();
            LogMessage($"Supported commandclasses:\n{string.Join("\n", supportedCommandClasses.Cast<object>())}");

            var manufacturerSpecific = motionSensor.GetCommandClass<ManufacturerSpecific>();
            var manufacturerSpecificReport = await manufacturerSpecific.Get();
            LogMessage($"Manufacturer specific report of Node {manufacturerSpecificReport.Node:D3} is [{manufacturerSpecificReport}]");

            var battery = motionSensor.GetCommandClass<Battery>();
            var batteryReport = await battery.Get();
            LogMessage($"Battery report of Node {batteryReport.Node:D3} is [{batteryReport}]");

            var sensorMultiLevel = motionSensor.GetCommandClass<SensorMultiLevel>();
            var sensorMultiLevelReport = await sensorMultiLevel.Get();
            LogMessage($"SensorMultiLevel report of Node {sensorMultiLevelReport.Node:D3} is [{sensorMultiLevelReport}]");

            var wakeUp = motionSensor.GetCommandClass<WakeUp>();
            var wakeUpReport = await wakeUp.GetInterval();
            LogMessage($"WakeUp report of Node {wakeUpReport.Node:D3} is [{wakeUpReport}]");

            var configuration = motionSensor.GetCommandClass<Configuration>();
            var configurationReport = await configuration.Get(64);
            LogMessage($"ConfigurationReport report of Node {configurationReport.Node:D3} is [{configurationReport}]");
            await configuration.Set(64, (long)TimeSpan.FromMinutes(1).TotalSeconds);
            Console.ReadLine();
        }
    }
}