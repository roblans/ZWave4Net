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

        public async Task<BatteryValue> Get()
        {
            var response = await Dispatcher.Send(new Command(ClassID, batteryCmd.Get), batteryCmd.Report);
            return BatteryValue.Parse(response.Payload);
        }
    }
}
