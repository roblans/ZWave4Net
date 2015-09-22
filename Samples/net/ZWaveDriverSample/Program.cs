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
                Console.WriteLine($"Version: {driver.Controller.GetVersion().Result}");
                Console.WriteLine($"HomeID: {driver.Controller.GetHomeID().Result:X}");
                Console.WriteLine($"ControllerID: {driver.Controller.GetNodeID().Result:D3}");
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
    }
}
