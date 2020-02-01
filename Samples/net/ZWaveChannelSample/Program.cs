using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZWave;
using ZWave.Channel;
using ZWave.CommandClasses;

namespace ZWaveChannelSample
{
    class Program
    {
        private const int _sirenNodeId = 21;

        private static async Task ConfigureSensor(ZWaveController controller)
        {
            NodeCollection nodes = await controller.DiscoverNodes();
            Node node = nodes[22];
            Node sirenNode = nodes[_sirenNodeId];

            sirenNode.MessageReceived += (_, _1) =>
            {
                Console.WriteLine("Sirane got message");
            };

            node.GetCommandClass<SensorBinary>().Changed += (a, b) =>
            {
                Console.WriteLine($"Current status: {b.Report.Value}");
            };

            using (AutoResetEvent initEvent = new AutoResetEvent(false))
            {
                Console.WriteLine("Waiting for signal");

                node.GetCommandClass<WakeUp>().Changed += (a, b) =>
                {
                    Console.WriteLine("Woke up");
                    initEvent.Set();
                };

                node.GetCommandClass<SensorBinary>().Changed += (a, b) =>
                {
                    Console.WriteLine("Changed state: " + b.Report.Value);
                };

                node.GetCommandClass<Basic>().Changed += (a, b) =>
                {
                    Console.WriteLine($"Got basic command - {b.Report.Value}");
                };

                node.MessageReceived += (a, b) =>
                {
                    Console.WriteLine("Got message");
                    initEvent.Set();
                };

                initEvent.WaitOne();
                VersionCommandClassReport[] supportedCommandClasses;
                for (int i = 0; i < 2; i++)
                    supportedCommandClasses = await node.GetSupportedCommandClasses();

                Association association = node.GetCommandClass<Association>();
                var groups = await association.GetGroups();
                /* await association.Add(0, _sirenNodeId);
                 await association.Add(1, 16);
                 await association.Add(2, _sirenNodeId);
                 await association.Add(3, _sirenNodeId);
                 await association.Add(4, _sirenNodeId);*/
                /*await association.Remove(1, _sirenNodeId);
                await association.Remove(2, _sirenNodeId);
                await association.Remove(3, _sirenNodeId);
                await association.Remove(4, _sirenNodeId);*/
                //var groups2 = await association.GetGroups();
                //await association.Remove(1, _sirenNodeId);
                /*await association.Add(1, _sirenNodeId);
                await association.Add(2, _sirenNodeId);
                await association.Add(3, _sirenNodeId);
                await association.Add(4, _sirenNodeId);*/
                await ClearNodeAssosiations(node);

                await association.Add(2, 1/*, _sirenNodeId*/);

                var group1 = await association.Get(1);
                var group2 = await association.Get(2);
                var group3 = await association.Get(3);
                var group4 = await association.Get(4);
                await node.HealNodeNetwork(CancellationToken.None);

               // await node.GetCommandClass<Configuration>().Set(2, (byte)255);
                /*var isOnValue = await node.GetCommandClass<Configuration>().Get(2);
                var isOnValue2 = await node.GetCommandClass<Configuration>().Get(1);
                await node.GetCommandClass<Configuration>().Set(1, (byte)0);
                var nibores = await node.GetNeighbours();*/


                Console.WriteLine("waiting 1 min");
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }

        private static async Task TestSirneSounds(ZWaveController controller)
        {
            NodeCollection nodes = await controller.DiscoverNodes();
            Node node = nodes[_sirenNodeId];
            await node.HealNodeNetwork(CancellationToken.None);
            var r = await node.GetSupportedCommandClasses();
            VersionCommandClassReport[] supportedCommandClasses = await node.GetSupportedCommandClasses();
            if (supportedCommandClasses.Any(cc => cc.Class == CommandClass.Battery))
            {
                var battery = node.GetCommandClass<Battery>();
                var batteryStatus = await battery.Get();
                Console.WriteLine($"Battery status is: {batteryStatus.Value}");
            }

            var configuration = node.GetCommandClass<Configuration>();
            SwitchBinary switchBinary = node.GetCommandClass<SwitchBinary>();
            for (int i = 1; i <= 10; i++)
            {
                Console.WriteLine($"Index: {i}");
                await configuration.Set(5, (byte)i);
                await switchBinary.Set(true);
                await Task.Delay(4000);
                await switchBinary.Set(false);
            }
        }

        private static async Task TestSirne(ZWaveController controller)
        {
            Console.WriteLine("Discovering nodes");
            NodeCollection nodes = await controller.DiscoverNodes();
            Node node = nodes[_sirenNodeId];
            VersionCommandClassReport[] supportedCommandClasses;
            Console.WriteLine("Getting supported command classes");
            supportedCommandClasses = await node.GetSupportedCommandClasses();

            Association association = node.GetCommandClass<Association>();
            var groups = await association.GetGroups();
            var group1 = await association.Get(1);
            var group2 = await association.Get(2);
            var group3 = await association.Get(3);
            var group0 = await association.Get(0);

            var isOnValue = await node.GetCommandClass<Configuration>().Get(2);
           // await node.GetCommandClass<Configuration>().Set(2, 2);
           // await node.GetCommandClass<Configuration>().Set(3, 2);
            await node.GetCommandClass<Basic>().Set(255);
            await node.GetCommandClass<Basic>().Set(0);

            var manufactore = await node.GetCommandClass<ManufacturerSpecific>().Get();

            Console.WriteLine();
        }

        private static async Task NeighboursTest(ZWaveController controller)
        {
            Console.WriteLine("Discovering nodes");
            NodeCollection nodes = await controller.DiscoverNodes();
            foreach (Node node in nodes)
            {
                int numberOfRetries = 2;
                Node[] neighbours = null;
                Console.WriteLine($"Discovering node {node.NodeID}");
                for (int i = 0; i < numberOfRetries; i++)
                {
                    try
                    {
                        var updateStatus = await node.HealNodeNetwork();
                        neighbours = await node.GetNeighbours();
                        break;
                    }
                    catch
                    {
                        Console.WriteLine($"Attempt {i + 1}/{numberOfRetries} failed.");
                    }
                }

                if (neighbours == null)
                {
                    Console.WriteLine($"Failed for node id {node.NodeID}");
                    continue;
                }

                Console.WriteLine($"[{node.NodeID}] - [{string.Join(", ", neighbours.Select(n => n.NodeID))}]");
            }
        }

        private static async Task TestWallPlug(ZWaveController controller)
        {
            Console.WriteLine("Discovering nodes");
            NodeCollection nodes = await controller.DiscoverNodes();
            Node node = nodes[16];
            Console.WriteLine("Changing state");
            SwitchBinary switchBinary = node.GetCommandClass<SwitchBinary>();
            var status = await switchBinary.Get();
            await switchBinary.Set(!status.Value);
            //await node.GetCommandClass<Basic>().Set(20);
        }

        private static async Task TestMultiSensor(ZWaveController controller)
        {
            Console.WriteLine("Discovering nodes");
            NodeCollection nodes = await controller.DiscoverNodes();
            Node node = nodes[20];
            var commandClasses = await node.GetSupportedCommandClasses();

            node.GetCommandClass<Basic>().Changed += (a, b) =>
            {
                Console.WriteLine($"Got basic command - {b.Report.Value}");
            };

            SensorMultiLevel sensorMultiLevel = node.GetCommandClass<SensorMultiLevel>();
           /* var supportedSensors = await sensorMultiLevel.GetSupportedSensors();
            foreach(var sensorType in supportedSensors.SupportedSensorTypes)
            {
                var sensorValue = await sensorMultiLevel.Get(sensorType);
                Console.WriteLine($"{sensorType} - {sensorValue.Value}{sensorValue.Unit}");
            }*/

            sensorMultiLevel.Changed += (a, b) =>
            {
                Console.WriteLine($"Change - {b.Report.Type} - {b.Report.Value}{b.Report.Unit}");
            };

            SensorBinary sensorBinary = node.GetCommandClass<SensorBinary>();
            var currentViaryValue = await sensorBinary.Get();
            Console.WriteLine($"Current binary value - {currentViaryValue.Value}");
            sensorBinary.Changed += (a, b) =>
            {
                Console.WriteLine($"Binary value - {b.Report.Value}");
            };

            Association association = node.GetCommandClass<Association>();
            var groups = await association.GetGroups();
            await ClearNodeAssosiations(node);
            var lifeline = await association.Get(1);
            await association.Add(1, 1/*, _sirenNodeId*/);

            var manufacture = await node.GetCommandClass<ManufacturerSpecific>().Get();

            node.MessageReceived += (a, b) =>
            {
                Console.WriteLine("Message recived");
            };

            Console.ReadLine();
        }

        static async Task Main(string[] args)
        {
            string portName = "COM5";

            ZWaveController controller = new ZWaveController(portName);
            Console.WriteLine("Opeing channel");
            controller.Open();

           // controller.Channel.Log = Console.Out;

            await NeighboursTest(controller);
            //await TestMultiSensor(controller);
            //await TestWallPlug(controller);
            //await TestSirneSounds(controller);
            //await TestSirne(controller);
            //await ConfigureSensor(controller);
            //await CloseSiren(controller);

            Console.WriteLine("Opeing channel");
            Console.ReadKey();
        }

        private static async Task CloseSiren(ZWaveController controller)
        {
            NodeCollection nodes = await controller.DiscoverNodes();
            Node node = nodes[_sirenNodeId];
            await node.GetCommandClass<Basic>().Set(0);
            //node.GetNeighbours()
        }

        private static async Task ClearNodeAssosiations(Node node)
        {
            Association association = node.GetCommandClass<Association>();
            var allAssosiationGroups = await association.GetGroups();
            for (byte i = 1; i <= allAssosiationGroups.GroupsSupported; i++)
            {
                var group = await association.Get(i);
                if (group.Nodes.Any())
                {
                    await association.Remove(i, group.Nodes);
                }
            }
        }
    }
}
