using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Devices.Fibaro
{
    public class PowerLoadEventArgs : EventArgs
    {
        public readonly float Value;

        public PowerLoadEventArgs(float value)
        {
            Value = value;
        }
    }
}
