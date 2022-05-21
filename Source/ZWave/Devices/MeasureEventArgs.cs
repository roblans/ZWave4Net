using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Devices
{
    public class MeasureEventArgs : EventArgs
    {
        public readonly Measure Meassure;

        public MeasureEventArgs(Measure meassure)
        {
            Meassure = meassure;
        }
    }
}
