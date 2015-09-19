using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave4Net.Communication;

namespace ZWave4Net.Commands
{
    public class Basic : CommandClass
    {
        public event EventHandler<ValueChangedEventArgs<byte>> Changed;

        enum command
        {
            Set = 0x01,
            Get = 0x02,
            Report = 0x03
        }

        public Basic(Node node)
            : base(0x20, node)
        {
        }
        
        protected void OnChanged(ValueChangedEventArgs<byte> e)
        {
            var handler = Changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public async Task<byte> Get()
        {
            var response = await Dispatcher.Send(new Command(ClassID, command.Get), command.Report);
            return response.Payload.First();
        }

        public Task Set(byte value)
        {
            return Dispatcher.Send(new Command(ClassID, command.Set, value));
        }

        protected override void OnEvent(Command command)
        {
            var value = command.Payload.First();
            OnChanged(new ValueChangedEventArgs<byte>(value));
        }
    }
}
