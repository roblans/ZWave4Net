using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net.Commands
{
    public class Alarm : CommandClass
    {
        enum alarmCmd
        {
            Get = 0x04,
            Report = 0x05,
            SupportedGet = 0x07,
            SupportedReport = 0x09,
        }

        public Alarm(Node node) : base(0x71, node)
        {
        }

        protected override Enum[] Commands
        {
            get { return Enum.GetValues(typeof(alarmCmd)).Cast<Enum>().ToArray(); }
        }

        public async Task<AlarmValue> GetValue()
        {
            var response = await Invoker.Invoke(new Command(ClassID, alarmCmd.Get));
            return AlarmValue.Parse(response.Payload);
        }

        protected override bool IsCorrelated(Enum request, Enum response)
        {
            if (object.Equals(request, alarmCmd.Get) && object.Equals(response, alarmCmd.Report))
                return true;
            if (object.Equals(request, alarmCmd.SupportedGet) && object.Equals(response, alarmCmd.SupportedReport))
                return true;

            return false;
        }

        protected override void OnEvent(Enum @event, byte[] payload)
        {
            var value = AlarmValue.Parse(payload);
            Platform.LogMessage(LogLevel.Debug, string.Format($"Event: Node = {Node}, Class = {ClassName}, Command = {@event}, {value}"));
        }
    }

}
