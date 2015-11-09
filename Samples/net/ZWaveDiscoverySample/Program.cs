using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave;
using ZWave.CommandClasses;

namespace ZWaveDiscoverySample
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
            lock (typeof(File))
            {
                if (Directory.Exists(@"D:\Temp"))
                {
                    File.AppendAllText(@"D:\Temp\ZWave.log", text + Environment.NewLine);
                }
            }
        }

        static private List<byte> unknownDevices;
        static private async Task Run(ZWaveController controller)
        {
            LogMessage($"Version: {await controller.GetVersion()}");
            LogMessage($"HomeID: {await controller.GetHomeID():X}");

            var controllerNodeID = await controller.GetNodeID();
            LogMessage($"ControllerID: {controllerNodeID:D3}");

            var nodes = await controller.GetNodes();
            unknownDevices = new List<byte>();
            unknownDevices.AddRange(nodes.Select(el => el.NodeID));

            foreach (var node in nodes.Where(el => el.NodeID != controllerNodeID))
            {
                try
                {
                    await RequestNodeType(node);
                }
                catch
                {
                    LogMessage($"Node: {node} not found, waiting for wake up event");
                    // subcribe to changes
                    Subscribe(node);
                }
            }

            LogMessage($"Waiting for wake up events from {unknownDevices.Count} unknown nodes");
            Console.ReadLine();
        }

        private static void Subscribe(Node node)
        {
            var wakeUp = node.GetCommandClass<WakeUp>();
            wakeUp.Changed += async (_, e) => 
            {
                try
                {
                    if (unknownDevices.Contains(node.NodeID))
                        await RequestNodeType(node);
                }
                catch { }
            };
        }

        private static async Task RequestNodeType(Node node)
        {
            var commandClass = node.GetCommandClass<ManufacturerSpecific>();
            var manInfo = await commandClass.Get();
                
            LogMessage($"Node: {node}, ManufacturerID = {manInfo.ManufacturerID}, ProductID = {manInfo.ProductID}, ProductType = {manInfo.ProductType} ");
            unknownDevices.Remove(node.NodeID);
        }
        
    }
}
