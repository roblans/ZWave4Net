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
