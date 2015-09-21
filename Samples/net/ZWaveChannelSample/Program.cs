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
            var portName = System.IO.Ports.SerialPort.GetPortNames().First();
            var channel = new ZWaveChannel(portName);

            channel.Open();
            try
            {
                var response = channel.Send(Function.GetVersion).Result;
                var data = response.TakeWhile(element => element != 0).ToArray();
                var version = Encoding.UTF8.GetString(data, 0, data.Length);
                Console.WriteLine($"Version: {version}");

                response = channel.Send(Function.MemoryGetId).Result;
                var homeID = BitConverter.ToUInt32(response.Take(4).Reverse().ToArray(), 0);
                Console.WriteLine($"HomeID: {homeID:X}");
                Console.WriteLine($"ControllerID: {response[4]:D3}");
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
    }
}
