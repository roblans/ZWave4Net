using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Devices
{
    public class SetpointEventArgs : EventArgs
    {
        public readonly Setpoint Setpoint;

        public SetpointEventArgs(Setpoint setpoint)
        {
            Setpoint = setpoint;
        }
    }
}
