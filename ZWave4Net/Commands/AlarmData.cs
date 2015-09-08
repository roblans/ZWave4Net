using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net.Commands
{
    public class AlarmData
    {
        public readonly AlarmType Type;
        public readonly byte Level;
        public readonly AlarmEvent Event;

        private AlarmData(AlarmType type, byte level, AlarmEvent @event)
        {
            Type = type;
            Level = level;
            Event = @event;
        }

        public override string ToString()
        {
            return string.Format($"Type = {Type}, Level = {Level}, Event = {Event}");
        }

        public static AlarmData Parse(byte[] data)
        {
            var type = (AlarmType)data[0];
            var level = data[1];
            var @event = (AlarmEvent)data[5];

            return new AlarmData(type, level, @event);
        }

    }
}
