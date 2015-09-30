using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Devices
{
    public class MeasureEventArgs : EventArgs
    {
        public readonly float Value;
        public readonly string Unit;

        public MeasureEventArgs(float value, string unit)
        {
            Value = value;
            Unit = unit;
        }
    }
}
