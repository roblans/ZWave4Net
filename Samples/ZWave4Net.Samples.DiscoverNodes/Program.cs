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
            // set threshold for logmessages, change to Info or Debug to get detailed logging
            Logger.LogThreshold = LogLevel.Warn;
            
            // redirect loggger
            Platform.LogMessage = Logger.LogMessage;

            // check if there are serial ports available
            if (System.IO.Ports.SerialPort.GetPortNames().Count() == 0)
                throw new Exception("No serial port available");

            //  default name of serialport
            var portName = "COM1";

            // only one port available?
            if (System.IO.Ports.SerialPort.GetPortNames().Count() == 1)
            {
                // yes, so use that port
                portName = System.IO.Ports.SerialPort.GetPortNames().First();
            }

            // run and wait
            Run(portName).Wait();
        }

        static async Task Run(string portName)
        {
            // create the serialport
            var port = new SerialPort(portName);

            // create the driver
            var driver = new ZWaveDriver(port);

            // open the driver
            await driver.Open();
            try
            {
                // get Version and HomeID/NetworkID 
                Console.WriteLine($"Version: {await driver.GetVersion()}");
                Console.WriteLine($"HomeID: {await driver.GetHomeID():X}");

                // start the discovery process
                await driver.DiscoverNodes();

                // wait for the discovery process to complete and get the nodes
                foreach (var node in await driver.GetNodes())
                {
                    // get protocolinfo from node
                    var protocolInfo = await node.GetNodeProtocolInfo();
                    
                    // dump node
                    Console.WriteLine($"Node: {node}, Generic = {protocolInfo.GenericType}, Basic = {protocolInfo.BasicType}, Listening = {protocolInfo.IsListening} ");
                }

                // wait for return
                Console.ReadLine();
            }
            catch(Exception ex)
            {
                Platform.LogMessage(LogLevel.Error, ex.Message);
            }
            finally
            {
                // and finally close the driver
                await driver.Close();
            }
        }
    }
}