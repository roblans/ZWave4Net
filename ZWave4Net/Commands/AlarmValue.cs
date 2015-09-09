using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net.Commands
{
    public enum AlarmType : byte
    {
        Unknown = 0,
        Smoke = 1,
        CarbonMonoxide = 2,
        CarbonDioxide = 3,
        Heat = 4,
        Flood = 5,
        AccessControl = 6,
        Burglar = 7,
        PowerManagement = 8,
        System = 9,
        Emergency = 10,
        Clock = 11,
        First = 255,
    };

    public enum AlarmDetailType : byte
    {
        Unknown = 0,
        Intrusion = 1,
        IntrusionUnknownLocation = 2,
        TamperingProductCoveringRemoved = 3,
        TamperingInvalidCode = 4,
        GlassBreakage = 5,
        GlassBreakageUnknownLocation = 6,
        MotionDetection = 7,
        MotionDetectionUnknownLocation = 8,
    };

    public class AlarmValue
    {
        public readonly AlarmType Type;
        public readonly byte Level;
        public readonly AlarmDetailType Detail;

        private AlarmValue(AlarmType type, byte level, AlarmDetailType detail)
        {
            Type = type;
            Level = level;
            Detail = detail;
        }

        public override string ToString()
        {
            return string.Format($"Type = {Type}, Level = {Level}, Detail = {Detail}");
        }

        public static AlarmValue Parse(byte[] data)
        {
            var type = (AlarmType)data[0];
            var level = data[1];
            var detail = (AlarmDetailType)data[5];

            return new AlarmValue(type, level, detail);
        }

    }
}
