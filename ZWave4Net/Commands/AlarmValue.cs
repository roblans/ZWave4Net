using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net.Commands
{

    public class AlarmValue
    {
        public AlarmType Type { get; private set; }
        public byte Level { get; private set; }
        public AlarmDetailType Detail { get; private set; }

        public override string ToString()
        {
            return string.Format($"Type = {Type}, Level = {Level}, Detail = {Detail}");
        }

        public static AlarmValue Parse(byte[] data)
        {
            var value = new AlarmValue();

            if (data.Length >= 7) 
            {
                // V2
                value.Level = data[3];
                value.Type = (AlarmType)data[4];
                value.Detail = (AlarmDetailType)data[5];
                var unknown = data[6];
            }
            else
            {
                // V1
                value.Type = (AlarmType)data[0];
                value.Level = data[1];
                var unknown = data[2];
            }

            return value;
        }

    }
}
