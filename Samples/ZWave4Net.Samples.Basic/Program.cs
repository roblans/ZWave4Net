using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave4Net;

namespace ZWave4Net.Samples.Basic
{
    class Program
    {
        static void Main(string[] args)
        {
            // set threshold for logmessages, change to Debug to get detailed logging
            Logger.LogThreshold = LogLevel.Info;

            // redirect loggger
            Platform.LogMessage = Logger.LogMessage;

            // check if there are serial ports available
            if (System.IO.Ports.SerialPort.GetPortNames().Count() == 0)
                throw new Exception("No serial port available");

            //  default name of serialport, change this to the correct portname if you have more than one serialport
            var portName = "COM1";

            // only one port available?
            if (System.IO.Ports.SerialPort.GetPortNames().Count() == 1)
            {
                // yes, so use that port
                portName = System.IO.Ports.SerialPort.GetPortNames().First();
            }

            // create the serialport
            var port = new SerialPort(portName);

            // run and wait
            Run(port).Wait();
        }

        static async Task Run(SerialPort port)
        {
            // create the driver
            var driver = new ZWaveDriver(port);

            // open the driver, this will start the discovery process
            await driver.Open();
            try
            {
                // get Version and HomeID/NetworkID 
                Platform.LogMessage(LogLevel.Info, string.Format($"Version: {await driver.GetVersion()}"));
                Platform.LogMessage(LogLevel.Info, string.Format($"HomeID: {await driver.GetHomeID():X}"));

                foreach(var node in await driver.GetNodes())
                {
                    var basic = node.GetCommandClass<ZWave4Net.Commands.Basic>();
                    try
                    {
                        var value = await basic.GetValue();
                        Platform.LogMessage(LogLevel.Info, string.Format($"Basic value of Node {node.NodeID} is {value}"));
                    }
                    catch (TimeoutException)
                    {
                        Platform.LogMessage(LogLevel.Info, string.Format($"Basic value of Node {node.NodeID} is not supported (is the node a controller or is the node sleeping?)"));
                        continue;
                    }
                }

                await Task.Run(() => Console.ReadLine());
            }
            catch (Exception ex)
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