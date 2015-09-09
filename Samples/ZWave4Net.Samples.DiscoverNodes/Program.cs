using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave4Net;

namespace ZWave4Net.Samples.DiscoverNodes
{
    class Program
    {
        static void Main(string[] args)
        {
            Platform.Log = Logger.LogMessage;

            var port = new SerialPort(System.IO.Ports.SerialPort.GetPortNames().First());
            var driver = new ZWaveDriver(port);

            driver.Open().Wait();
            try
            {
                Console.ReadLine();
            }
            finally
            {
                driver.Close();
            }
        }
    }
}
