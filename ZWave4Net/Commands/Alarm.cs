using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave4Net.Communication;

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

        public async Task<AlarmValue> Get()
        {
            var response = await Dispatcher.Send(new Command(ClassID, alarmCmd.Get), alarmCmd.Report);
            return AlarmValue.Parse(response.Payload);
        }

        protected override void OnEvent(Enum command, byte[] payload)
        {
            var value = AlarmValue.Parse(payload);
            Platform.LogMessage(LogLevel.Debug, string.Format($"Event: Node = {Node}, Class = {ClassName}, Command = {command}, {value}"));
        }
    }

}
