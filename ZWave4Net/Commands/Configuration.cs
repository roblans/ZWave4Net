using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave4Net.Communication;

namespace ZWave4Net.Commands
{
    public class Configuration : CommandClass
    {
        enum configurationCmd
        {
            Set = 0x04,
            Get = 0x05,
            Report = 0x06
        }

        public Configuration(Node node) : base(0x70, node)
        {
        }

        protected override Enum[] Commands
        {
            get { return Enum.GetValues(typeof(configurationCmd)).Cast<Enum>().ToArray(); }
        }

        public Task SetValue(byte parameter, byte value)
        {
            return Invoker.Post((new Command(ClassID, configurationCmd.Set, parameter, 1, value)));
        }


        public async Task<byte> GetValue(byte parameter)
        {
            var response = await Invoker.Send(new Command(ClassID, configurationCmd.Get, parameter), configurationCmd.Report);
            var length = response.Payload[1];
            if (length == 1)
            {
                return response.Payload[2];
            }
            throw new InvalidCastException();
        }
    }
}
