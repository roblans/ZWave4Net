using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave4Net;
using ZWave4Net.Commands;

namespace ZWave4Net.Samples.DiscoverNodes
{
    class Program
    {
        static void Main(string[] args)
        {
            // set threshold for logmessages, change to Debug to get detailed logging
            Logger.LogThreshold = LogLevel.Debug;
            
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

                // start the discovery process
                driver.DiscoverNodes();

                // wait for the discovery process to complete and get the nodes
                foreach (var node in await driver.GetNodes())
                {
                    // get protocolinfo from node
                    var protocolInfo = await node.GetNodeProtocolInfo();

                    // dump node
                    Platform.LogMessage(LogLevel.Info, string.Format($"Node: {node}, Generic = {protocolInfo.GenericType}, Basic = {protocolInfo.BasicType}, Listening = {protocolInfo.IsListening} "));
                }

                var wallPlug = (await driver.GetNodes()).First(element => element.NodeID == 6);
                var configuration = wallPlug.GetCommandClass<Configuration>();

                for (int i = 0; i < 2; i++)
                {
                    Platform.LogMessage(LogLevel.Warn, "GetValue");
                    var c = await configuration.GetValue(62);
                    Platform.LogMessage(LogLevel.Warn, "SetValue");
                    await configuration.SetValue(62, c);
                    Platform.LogMessage(LogLevel.Warn, "End");
                    await Task.Delay(1000);
                }
                //for(var i=1; i < 9; i++)
                //{
                //    await configuration.SetValue(62, (byte)i);
                //    await Task.Delay(1000);
                //}

                //var switchBinary = wallPlug.GetCommandClass<SwitchBinary>();
                //await switchBinary.SetValue(BinarySwitchValue.On);
                //await Task.Delay(1000);
                //await switchBinary.SetValue(BinarySwitchValue.Off);
                //await Task.Delay(1000);
                //await switchBinary.SetValue(BinarySwitchValue.On);
                //await Task.Delay(1000);
                //await switchBinary.SetValue(BinarySwitchValue.Off);

                var associaton = wallPlug.GetCommandClass<Association>();
                //await associaton.Add(1, 1);
                //await associaton.Add(2, 1);
                //await associaton.Add(3, 1);
                //var a1 = await associaton.Get(1);
                //var a2 = await associaton.Get(2);
                //var a3 = await associaton.Get(3);
                //await associaton.Remove(1, 1);
                await Task.Run(() => Console.ReadLine());
            }
            catch(Exception ex)
            {
                Platform.LogMessage(LogLevel.Error, ex.Message);
                await Task.Run(() => Console.ReadLine());
            }
        }
    }
}