using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var response = await Invoker.Invoke(new Command(ClassID, batteryCmd.Get));
            return ParseLevel(response.Payload);
        }

        protected override bool IsCorrelated(Enum request, Enum response)
        {
            if (object.Equals(request, batteryCmd.Get) && object.Equals(response, batteryCmd.Report))
                return true;

            return false;
        }

        protected override void OnResponse(Enum response, byte[] payload)
        {
            var level = ParseLevel(payload);
            Platform.Log(LogLevel.Info, string.Format("Response: Node = {0}, Class = {1}, Command = {2}, Level = {3}", Node, ClassName, response, level));
        }

        protected override void OnEvent(Enum @event, byte[] payload)
        {
            var level = ParseLevel(payload);
            Platform.Log(LogLevel.Info, string.Format("Event: Node = {0}, Class = {1}, Command = {2}, Level = {3}", Node, ClassName, @event, level));
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
