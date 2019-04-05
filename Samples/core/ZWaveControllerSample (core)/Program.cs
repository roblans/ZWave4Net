using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ZWave;
using ZWave.Channel;
using ZWave.CommandClasses;

namespace ZWaveControllerSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var portName = RJCP.IO.Ports.SerialPortStream.GetPortNames().Where(element => element != "COM1").First();

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
                    LogMessage($"{inner}");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"{ex}");
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
            lock (typeof(File))
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

            var controllerNodeID = await controller.GetNodeID();
            LogMessage($"ControllerID: {controllerNodeID:D3}");

            var nodes = await controller.GetNodes();
            foreach (var node in nodes)
            {
                var protocolInfo = await node.GetProtocolInfo();
                LogMessage($"Node: {node}, Generic = {protocolInfo.GenericType}, Basic = {protocolInfo.BasicType}, Listening = {protocolInfo.IsListening} ");

                var neighbours = await node.GetNeighbours();
                LogMessage($"Node: {node}, Neighbours = {string.Join(", ", neighbours.Cast<object>().ToArray())}");

                // subcribe to changes
                Subscribe(node);
            }

            Console.ReadLine();
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
            wakeUp.Changed += (_, e) => { LogMessage($"WakeUp report of Node {e.Report.Node:D3} changed to [{e.Report}]"); };

            var switchBinary = node.GetCommandClass<SwitchBinary>();
            switchBinary.Changed += (_, e) => LogMessage($"SwitchBinary report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var thermostatSetpoint = node.GetCommandClass<ThermostatSetpoint>();
            thermostatSetpoint.Changed += (_, e) => LogMessage($"thermostatSetpoint report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var sceneActivation = node.GetCommandClass<SceneActivation>();
            sceneActivation.Changed += (_, e) => LogMessage($"sceneActivation report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var multiChannel = node.GetCommandClass<MultiChannel>();
            multiChannel.Changed += (_, e) => LogMessage($"multichannel report of Node {e.Report.Node:D3} changed to [{e.Report}]");
        }
    }
}
