using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave4Net;
using ZWave4Net.Commands;

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


                var nodes = await driver.GetNodes();
                foreach (var node in nodes)
                {
                    // get protocolinfo from node
                    var protocolInfo = await node.GetNodeProtocolInfo();

                    // dump node
                    Platform.LogMessage(LogLevel.Info, string.Format($"Node: {node}, Generic = {protocolInfo.GenericType}, Basic = {protocolInfo.BasicType}, Listening = {protocolInfo.IsListening} "));
                }

                // get the wallplug
                var wallPlug = nodes[6];
                // turn it off
                await wallPlug.GetCommandClass<SwitchBinary>().SetValue(BinarySwitchValue.Off);
                // set on color to green
                await wallPlug.GetCommandClass<Configuration>().SetValue(61, 4);
                // set on off to none
                await wallPlug.GetCommandClass<Configuration>().SetValue(62, 8);

                // get the motion sensor
                var motionSensor = nodes[3];
                // subscribe to basic value changed event
                motionSensor.GetCommandClass<Commands.Basic>().ValueChanged += async (_, e) =>
                {
                    Platform.LogMessage(LogLevel.Info, string.Format($"MotionSensor changed to {e.Value}"));
                    if (e.Value == 0xFF)
                    {
                        Platform.LogMessage(LogLevel.Info, "Set wallplug on");
                        await wallPlug.GetCommandClass<SwitchBinary>().SetValue(BinarySwitchValue.On);
                    }
                };

                // subscribe to basic value changed event
                wallPlug.GetCommandClass<Commands.Basic>().ValueChanged += (_, e) =>
                {
                    Platform.LogMessage(LogLevel.Info, string.Format($"Wallplug changed to {e.Value}"));
                };

                await Task.Run(() => Console.ReadLine());
            }
            catch (Exception ex)
            {
                Platform.LogMessage(LogLevel.Error, ex.Message);
                await Task.Run(() => Console.ReadLine());
            }
        }
    }
}