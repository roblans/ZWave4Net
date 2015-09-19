using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave4Net.Communication;

namespace ZWave4Net.Commands
{
    public class WakeUp : CommandClass
    {
        enum wakeUpCmd
        {
            IntervalSet = 0x04,
            IntervalGet = 0x05,
            IntervalReport = 0x06,
            Notification = 0x07,
            NoMoreInformation = 0x08
        }

        public WakeUp(Node node) : base(0x84, node)
        {
        }

        public async Task<byte> GetInterval()
        {
            var response = await Dispatcher.Send(new Command(ClassID, wakeUpCmd.IntervalGet), wakeUpCmd.IntervalReport);
            return response.Payload.First();
        }

        public Task SetInterval(byte value)
        {
            return Dispatcher.Send(new Command(ClassID, wakeUpCmd.IntervalSet, value));
        }

        protected override void OnEvent(Command command)
        {
        }
    }
}
