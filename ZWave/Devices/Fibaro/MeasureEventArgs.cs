using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Devices.Fibaro
{
    public class MeasureEventArgs : EventArgs
    {
        public readonly float Value;

        public MeasureEventArgs(float value)
        {
            Value = value;
        }
    }
}
