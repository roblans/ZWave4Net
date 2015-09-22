using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using ZWave.Driver.Communication;

namespace ZWave.Driver
{
    public class Controller
    {
        public readonly ZWaveDriver Driver;

        public Controller(ZWaveDriver driver)
        {
            Driver = driver;
        }

        public ZWaveChannel Channel
        {
            get { return Driver.Channel; }
        }

        public async Task<string> GetVersion()
        {
            var response = await Channel.Send(Function.GetVersion);
            var data = response.TakeWhile(element => element != 0).ToArray();
            return Encoding.UTF8.GetString(data, 0, data.Length);
        }

        public async Task<uint> GetHomeID()
        {
            var response = await Channel.Send(Function.MemoryGetId);
            return ByteConverter.ToUInt32(response);
        }

        public async Task<byte> GetControllerID()
        {
            var response = await Channel.Send(Function.MemoryGetId);
            return response[4];
        }

    }
}
