using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Devices.Fibaro
{
    public class EnergyConsumptionEventArgs : EventArgs
    {
        public readonly float Value;

        public EnergyConsumptionEventArgs(float value)
        {
            Value = value;
        }
    }
}
