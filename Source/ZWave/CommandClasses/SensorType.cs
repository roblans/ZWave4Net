using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.CommandClasses
{
    public enum SensorType : byte
    {
        Undefined = 0x00,
        Temperature = 0x01,
        General = 0x02,
        Luminance = 0x03,
        Power = 0x04,
        RelativeHumidity = 0x05,
        Velocity = 0x06,
        Direction = 0x07,
        AtmosphericPressure = 0x08,
        BarometricPressure = 0x09,
        SolarRadiation = 0x0A,
        DewPoint = 0x0B,
        RainRate = 0x0C,
        TideLevel = 0x0D,
        Weight = 0x0E,
        Voltage = 0x0F,
        Current = 0x10,
        CO2 = 0x11,
        AirFlow = 0x12,
        TankCapacity = 0x13,
        Distance = 0x14,
        AnglePosition = 0x15,
        Rotation = 0x16,
        WaterTemperature = 0x17,
        SoilTemperature = 0x18,
        SeismicIntensity = 0x19,
        SeismicMagnitude = 0x1A,
        Ultraviolet = 0x1B,
        ElectricalResistivity = 0x1C,
        ElectricalConductivity = 0x1D,
        Loudness = 0x1E,
        Moisture = 0x1F,
    };
}
