using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave.Driver;

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

            var nodes = await driver.GetNodes();
            foreach(var node in nodes)
            {
                Console.WriteLine(string.Format($"Node: {node}"));
            }
        }
    }
}