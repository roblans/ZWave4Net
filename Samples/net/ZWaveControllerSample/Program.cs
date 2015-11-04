﻿using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZWave;
using ZWave.CommandClasses;

namespace ZWaveDriverSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var portName = System.IO.Ports.SerialPort.GetPortNames().Where(element =>
                element != "COM1"
                && element != "COM10"
                && element != "COM11").First();

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

            var controllerNodeID = await controller.GetNodeID();
            LogMessage($"ControllerID: {controllerNodeID:D3}");

            var nodes = await controller.GetNodes();
            //foreach (var node in nodes)
            //{
            //    var protocolInfo = await node.GetProtocolInfo();
            //    LogMessage($"Node: {node}, Generic = {protocolInfo.GenericType}, Basic = {protocolInfo.BasicType}, Listening = {protocolInfo.IsListening} ");

            //    var neighbours = await node.GetNeighbours();
            //    LogMessage($"Node: {node}, Neighbours = {string.Join(", ", neighbours.Cast<object>().ToArray())}");

            //    // subcribe to changes
            //    Subscribe(node);
            //}


            //await InitializeWallPlug(nodes[2]);
            //await InitializeWallPlug(nodes[3]);
            //await InitializeShockSensor(nodes[4]);
            //await InitializeGarageDoorSensor(nodes[5]);
            //await InitializeThermostat(nodes[6]);
            //await InitializeMultiSensor(nodes[7]);
            //await InitializeDoorSensor(nodes[8]);
            await InitializeFibaro2xSwitch(nodes[4]);

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
            wakeUp.Changed += (_, e) =>{  LogMessage($"WakeUp report of Node {e.Report.Node:D3} changed to [{e.Report}]"); };

            var switchBinary = node.GetCommandClass<SwitchBinary>();
            switchBinary.Changed += (_, e) => LogMessage($"SwitchBinary report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var thermostatSetpoint = node.GetCommandClass<ThermostatSetpoint>();
            thermostatSetpoint.Changed += (_, e) => LogMessage($"thermostatSetpoint report of Node {e.Report.Node:D3} changed to [{e.Report}]");
        }

        private static async Task InitializeWallPlug(Node node)
        {
            var association = node.GetCommandClass<Association>();
            await association.Add(1, 1);
            await association.Add(2, 1);
            await association.Add(3, 1);

            var supportedCommandClasses = await node.GetSupportedCommandClasses();
            LogMessage($"Supported commandclasses:\n{string.Join("\n", supportedCommandClasses.Cast<object>())}");

            var manufacturerSpecific = node.GetCommandClass<ManufacturerSpecific>();
            var manufacturerSpecificReport = await manufacturerSpecific.Get();
            LogMessage($"Manufacturer specific report of Node {manufacturerSpecificReport.Node:D3} is [{manufacturerSpecificReport}]");

            var configuration = node.GetCommandClass<Configuration>();
            await configuration.Set(61, (byte)2); // on => White
            await configuration.Set(62, (byte)8); // off => None
            await configuration.Set(47, (ushort)900); // measure interval 15 minutes

            var switchBinary = node.GetCommandClass<SwitchBinary>();
            await switchBinary.Set(true);

            Console.ReadLine();
        }

        private static async Task InitializeShockSensor(Node node)
        {
            LogMessage("Please wakeup the shock sensor.");
            Console.ReadLine();

            var association = node.GetCommandClass<Association>();
            await association.Add(1, 1);
            await association.Add(2, 1);
            await association.Add(3, 1);

            var supportedCommandClasses = await node.GetSupportedCommandClasses();
            LogMessage($"Supported commandclasses:\n{string.Join("\n", supportedCommandClasses.Cast<object>())}");

            var manufacturerSpecific = node.GetCommandClass<ManufacturerSpecific>();
            var manufacturerSpecificReport = await manufacturerSpecific.Get();
            LogMessage($"Manufacturer specific report of Node {manufacturerSpecificReport.Node:D3} is [{manufacturerSpecificReport}]");

            var battery = node.GetCommandClass<Battery>();
            var batteryReport = await battery.Get();
            LogMessage($"Battery report of Node {batteryReport.Node:D3} is [{batteryReport}]");

            var wakeUp = node.GetCommandClass<WakeUp>();
            await wakeUp.SetInterval(TimeSpan.FromMinutes(15), 1);
            var wakeUpReport = await wakeUp.GetInterval();
            LogMessage($"WakeUp report of Node {wakeUpReport.Node:D3} is [{wakeUpReport}]");

            Console.ReadLine();
        }

        private static async Task InitializeGarageDoorSensor(Node node)
        {
            LogMessage("Please wakeup the garagedoor sensor.");
            Console.ReadLine();

            var association = node.GetCommandClass<Association>();
            await association.Add(1, 1);
            await association.Add(2, 1);
            await association.Add(3, 1);

            var supportedCommandClasses = await node.GetSupportedCommandClasses();
            LogMessage($"Supported commandclasses:\n{string.Join("\n", supportedCommandClasses.Cast<object>())}");

            var manufacturerSpecific = node.GetCommandClass<ManufacturerSpecific>();
            var manufacturerSpecificReport = await manufacturerSpecific.Get();
            LogMessage($"Manufacturer specific report of Node {manufacturerSpecificReport.Node:D3} is [{manufacturerSpecificReport}]");

            var battery = node.GetCommandClass<Battery>();
            var batteryReport = await battery.Get();
            LogMessage($"Battery report of Node {batteryReport.Node:D3} is [{batteryReport}]");

            var wakeUp = node.GetCommandClass<WakeUp>();
            await wakeUp.SetInterval(TimeSpan.FromMinutes(15), 1);
            var wakeUpReport = await wakeUp.GetInterval();
            LogMessage($"WakeUp report of Node {wakeUpReport.Node:D3} is [{wakeUpReport}]");

            Console.ReadLine();
        }

        private static async Task InitializeMultiSensor(Node motionSensor)
        {
            LogMessage("Please wakeup the multisensor.");
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

            var wakeUp = motionSensor.GetCommandClass<WakeUp>();
            await wakeUp.SetInterval(TimeSpan.FromMinutes(15), 0x01);
            var wakeUpReport = await wakeUp.GetInterval();
            LogMessage($"WakeUp report of Node {wakeUpReport.Node:D3} is [{wakeUpReport}]");

            var configuration = motionSensor.GetCommandClass<Configuration>();
            await configuration.Set(111, (uint)240); // minimum interval 240 seconds

            Console.ReadLine();
        }

        public static async Task InitializeThermostat(Node node)
        {
            LogMessage("Please wakeup the thermostat.");
            Console.ReadLine();

            var battery = node.GetCommandClass<Battery>();
            var batteryReport = await battery.Get();
            LogMessage($"Battery report of Node {batteryReport.Node:D3} is [{batteryReport}]");

            var wakeUp = node.GetCommandClass<WakeUp>();
            var wakeUpReport = await wakeUp.GetInterval();
            await wakeUp.SetInterval(TimeSpan.FromMinutes(15), 1);
            LogMessage($"WakeUp report of Node {wakeUpReport.Node:D3} is [{wakeUpReport}]");

            var thermostatSetpoint = node.GetCommandClass<ThermostatSetpoint>();
            var thermostatSetpointReport = await thermostatSetpoint.Get(ThermostatSetpointType.Heating);
            LogMessage($"SetpointReport report of Node {thermostatSetpointReport.Node:D3} is [{thermostatSetpointReport}]");
            //await thermostatSetpoint.Set(ThermostatSetpointType.Heating, 18.0F);

            var clock = node.GetCommandClass<Clock>();
            var clockReport = await clock.Get();
            LogMessage($"clockReport report of Node {clockReport.Node:D3} is [{clockReport}]");
        }

        private static async Task InitializeDoorSensor(Node node)
        {
            LogMessage("Please wakeup the door sensor.");
            Console.ReadLine();

            var association = node.GetCommandClass<Association>();
            await association.Add(1, 1);
            await association.Add(2, 1);
            await association.Add(3, 1);

            var supportedCommandClasses = await node.GetSupportedCommandClasses();
            LogMessage($"Supported commandclasses:\n{string.Join("\n", supportedCommandClasses.Cast<object>())}");

            var manufacturerSpecific = node.GetCommandClass<ManufacturerSpecific>();
            var manufacturerSpecificReport = await manufacturerSpecific.Get();
            LogMessage($"Manufacturer specific report of Node {manufacturerSpecificReport.Node:D3} is [{manufacturerSpecificReport}]");

            var battery = node.GetCommandClass<Battery>();
            var batteryReport = await battery.Get();
            LogMessage($"Battery report of Node {batteryReport.Node:D3} is [{batteryReport}]");

            var wakeUp = node.GetCommandClass<WakeUp>();
            await wakeUp.SetInterval(TimeSpan.FromMinutes(15), 1);
            var wakeUpReport = await wakeUp.GetInterval();
            LogMessage($"WakeUp report of Node {wakeUpReport.Node:D3} is [{wakeUpReport}]");

            var basic = node.GetCommandClass<Basic>();
            var basicReport = await basic.Get();
            LogMessage($"BasicReport report of Node {basicReport.Node:D3} is [{basicReport}]");

            var alarm = node.GetCommandClass<Alarm>();
            var alarmReport = await basic.Get();
            LogMessage($"AlarmReport report of Node {alarmReport.Node:D3} is [{alarmReport}]");

            Console.ReadLine();
        }

        private static async Task InitializeFibaro2xSwitch(Node node)
        {
            await Task.Run(() =>
            {
                var multiChannel = node.GetCommandClass<MultiChannel>();
                multiChannel.Changed += (_, a) =>
                {
                    LogMessage($"Event received:\n{a.Report}");
                };
            });

            Console.ReadLine();
        }
    }
}