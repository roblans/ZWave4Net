using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Devices
{
    public class Measure
    {
        public readonly float Value;
        public readonly Unit Unit;
        public readonly string Symbol;

        public Measure(float value, Unit unit)
        {
            Value = value;
            Unit = unit;
            Symbol = unit.GetSymbol();
        }

        public override string ToString()
        {
            return $"{Value} {Symbol}";
        }
    }
}
