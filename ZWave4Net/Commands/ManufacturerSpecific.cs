using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave4Net.Communication;

namespace ZWave4Net.Commands
{
    public class ManufacturerSpecific : CommandClass
    {
        enum manufacturerSpecificCmd
        {
            Get = 0x04,
            Report = 0x05
        }

        public ManufacturerSpecific(Node node) : base(0x72, node)
        {
        }

        public async Task<ManufacturerSpecificValue> Get()
        {
            var response = await Dispatcher.Send(new Command(ClassID, manufacturerSpecificCmd.Get), manufacturerSpecificCmd.Report);
            return ManufacturerSpecificValue.Parse(response.Payload);
        }
    }
}
