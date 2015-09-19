using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave4Net.Communication;

namespace ZWave4Net.Commands
{
    public class Battery : CommandClass
    {
        enum batteryCmd
        {
            Get = 0x02,
            Report = 0x03
        }

        public Battery(Node node) : base(0x80, node)
        {
        }

        protected override Enum[] Commands
        {
            get { return Enum.GetValues(typeof(batteryCmd)).Cast<Enum>().ToArray(); }
        }

        public async Task<BatteryLevel> Get()
        {
            var response = await Dispatcher.Send(new Command(ClassID, batteryCmd.Get), batteryCmd.Report);
            return ParseLevel(response.Payload);
        }

        private BatteryLevel ParseLevel(byte[] payload)
        {
            var value = payload.First();
            if (value == 0xFF)
            {
                value = 0;
            }
            return new BatteryLevel(value);
        }
    }
}
