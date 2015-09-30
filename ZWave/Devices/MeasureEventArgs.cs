using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Devices
{
    public class MeasureEventArgs : EventArgs
    {
        public readonly float Value;
        public readonly string Symbol;

        public MeasureEventArgs(float value, string symbol)
        {
            Value = value;
            Symbol = symbol;
        }

        public override string ToString()
        {
            return $"{Value} {Symbol}";
        }
    }
}
