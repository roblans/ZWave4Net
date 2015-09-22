using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave.Driver.Communication;

namespace ZWaveChannelSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // use first serial port
            var portName = System.IO.Ports.SerialPort.GetPortNames().First();

            // create a channel
            var channel = new ZWaveChannel(portName);

            // uncommment to see detailed logging
            // channel.Log = Console.Out;

            // subcribe to node events
            channel.NodeEventReceived += (sender, e) => Console.WriteLine($"Event: NodeID:{e.NodeID:D3} Command:[{e.Command}]");

            // open channel
            channel.Open();
            try
            {
                Run(channel).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            finally
            {
                Console.ReadLine();
                channel.Close();
            }
        }

        static private async Task Run(ZWaveChannel channel)
        {
            // ZWave function: GetVersion
            var response = await channel.Send(Function.GetVersion);
            var data = response.TakeWhile(element => element != 0).ToArray();
            var version = Encoding.UTF8.GetString(data, 0, data.Length);
            Console.WriteLine($"Version: {version}");

            // ZWave function: MemoryGetId
            response = await channel.Send(Function.MemoryGetId);
            var homeID = BitConverter.ToUInt32(response.Take(4).Reverse().ToArray(), 0);
            Console.WriteLine($"HomeID: {homeID:X}");
            Console.WriteLine($"ControllerID: {response[4]:D3}");

            // NodeID of the fibaro wall plug
            byte wallPlugID = 6;

            // turn wallplug on
            Console.WriteLine($"Set wallplug on.");
            await channel.Send(wallPlugID, new Command(CommandClass.SwitchBinary, 0x01, 255));

            await Task.Delay(1000);

            // turn wallplug off
            Console.WriteLine($"Set wallplug off.");
            await channel.Send(wallPlugID, new Command(CommandClass.SwitchBinary, 0x01, 0));

        }
    }
}
