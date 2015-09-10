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
            var portName = "COM5";

            // only one port available?
            if (System.IO.Ports.SerialPort.GetPortNames().Count() == 1)
            {
                // yes, so use that port
                portName = System.IO.Ports.SerialPort.GetPortNames().First();
            }

            // create the serialport
            var port = new SerialPort(portName);

            // create the driver
            var driver = new ZWaveDriver(port);

            // open the driver
            driver.Open();
            try
            {
                // run and wait
                Run(driver).Wait();
            }
            finally
            {
                // close the driver
                driver.Close();
            }
        }

        static async Task Run(ZWaveDriver driver)
        {
            try
            {
                // get Version and HomeID/NetworkID 
                Platform.LogMessage(LogLevel.Info, string.Format($"Version: {await driver.GetVersion()}"));
                Platform.LogMessage(LogLevel.Info, string.Format($"HomeID: {await driver.GetHomeID():X}"));
                Platform.LogMessage(LogLevel.Info, string.Format($"ControllerID: {await driver.GetControllerID():D3}"));

                Platform.LogMessage(LogLevel.Info, "Enter the ID of a node");
                var input = await Task.Run(() => Console.ReadLine());
                var nodeID = byte.Parse(input);

                var node = (await driver.GetNodes()).SingleOrDefault(element => element.NodeID == nodeID);
                if (node == null)
                {
                    Platform.LogMessage(LogLevel.Error, string.Format($"Error: Node {nodeID} does not exists."));
                    return;
                }

                var basic = node.GetCommandClass<ZWave4Net.Commands.Basic>();
                basic.ValueChanged += (_, e) =>
                {
                    Platform.LogMessage(LogLevel.Info, string.Format($"Value changed to {e.Value}"));
                };
                Platform.LogMessage(LogLevel.Info, string.Format($"Subscribed to ValueChanged event"));

                try
                {
                    var value = await basic.GetValue();
                    Platform.LogMessage(LogLevel.Info, string.Format($"Value is {value}"));
                }
                catch (TimeoutException)
                {
                    Platform.LogMessage(LogLevel.Info, string.Format($"Get current value failed. (is the node a controller or is the node sleeping?)"));
                }
                await Task.Run(() => Console.ReadLine());
            }
            catch (Exception ex)
            {
                Platform.LogMessage(LogLevel.Error, ex.Message);
            }
        }
    }
}