using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.CommandClasses
{
    public enum ElectricMeterScale
    {
        kWh  = 0,
        kVAh = 1,
        W = 2,
        PulseCount = 3,
        V = 4,
        A = 5,
        PowerFactor = 6,
    }

    public enum GasMeterScale
    {
        CubicMeters = 0,
        CubicFeet = 1,
        PulseCount = 3,
    }

    public enum WaterMeterScale
    {
        CubicMeters = 0,
        CubicFeet = 1,
        USGallons = 2,
        PulseCount = 3,
    }
}
