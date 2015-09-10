using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net.Commands
{
    public class SwitchBinary : CommandClass
    {
        enum switchBinaryCmd
        {
            Set = 0x01,
            Get = 0x02,
            Report = 0x03
        }

        public SwitchBinary(Node node) : base(0x25, node)
        {
        }

        protected override Enum[] Commands
        {
            get { return Enum.GetValues(typeof(switchBinaryCmd)).Cast<Enum>().ToArray(); }
        }

        protected override bool IsCorrelated(Enum request, Enum response)
        {
            if (object.Equals(request, switchBinaryCmd.Get) && object.Equals(response, switchBinaryCmd.Report))
                return true;

            return false;
        }

        public Task SetValue(BinarySwitchValue value)
        {
            return Invoker.Post(new Command(ClassID, switchBinaryCmd.Set, value == BinarySwitchValue.On ? (byte)0xFF : (byte)0x00));
        }

        public async Task<BinarySwitchValue> GetValue()
        {
            var response = await Invoker.Send(new Command(ClassID, switchBinaryCmd.Get));
            return response.Payload.First() == 0xFF ? BinarySwitchValue.On : BinarySwitchValue.Off;
        }

        public async Task ToggleValue()
        {
            var value = await GetValue().ConfigureAwait(false);
            await SetValue(value == BinarySwitchValue.On ? BinarySwitchValue.Off : BinarySwitchValue.On);
        }
    }
}
