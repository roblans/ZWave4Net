using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net.Commands
{
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
}
