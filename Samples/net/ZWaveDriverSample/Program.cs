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

            driver.Open();
            try
            {
                Run(driver).Wait();
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
            foreach(var node in nodes)
            {
                var protocolInfo = await node.GetNodeProtocolInfo();

                // dump node
                Console.WriteLine($"Node: {node}, Generic = {protocolInfo.GenericType}, Basic = {protocolInfo.BasicType}, Listening = {protocolInfo.IsListening} ");
            }
            Console.WriteLine();

            // NodeID of the fibaro wall plug
            byte wallPlugID = 6;

            var wallPlug = nodes[wallPlugID];
            var basic = wallPlug.GetCommandClass<Basic>();
            var report = await basic.Get();

            Console.WriteLine($"Basic report of Node {report.Node:D3} is {report}");
        }
    }
}