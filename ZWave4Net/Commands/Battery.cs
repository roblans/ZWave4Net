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

        public async Task<BatteryLevel> GetLevel()
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

        protected override void OnReport(Enum command, byte[] payload)
        {
            var level = ParseLevel(payload);
            Platform.LogMessage(LogLevel.Debug, string.Format($"Event: Node = {Node}, Class = {ClassName}, Command = {command}, Level = {level}"));
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
