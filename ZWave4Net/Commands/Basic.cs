using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net.Commands
{
    public class Basic : CommandClass
    {
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

        protected override Enum[] Commands
        {
            get { return Enum.GetValues(typeof(command)).Cast<Enum>().ToArray(); }
        }

        public async Task<byte> Get()
        {
            var response = await Invoker.Invoke(new Command(ClassID, command.Get));
            return response.Payload.First();
        }

        public Task Set(byte value)
        {
            return Invoker.Invoke(new Command(ClassID, command.Set, value));
        }

        protected override bool IsCorrelated(Enum request, Enum response)
        {
            return object.Equals(request, command.Get) && object.Equals(response, command.Report);
        }

        protected override void OnResponse(Enum response, byte[] payload)
        {
            var value = payload.First();
            Platform.Log(LogLevel.Info, string.Format($"Response: Node = {Node}, Class = {ClassName}, Command = {response}, Value = {value}"));
        }

        protected override void OnEvent(Enum @event, byte[] payload)
        {
            var value = payload.First();
            Platform.Log(LogLevel.Info, string.Format($"Event: Node = {Node}, Class = {ClassName}, Command = {@event}, Value = {value}"));
        }
    }
}
