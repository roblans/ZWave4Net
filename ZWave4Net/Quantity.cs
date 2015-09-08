using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net
{
    public class Quantity<T>
    {
        public readonly T Value;
        public readonly string Unit;

        public Quantity(T value, string unit)
        {
            Value = value;
            Unit = unit;
        }

        public string DisplayValue
        {
            get { return string.Format($"{Value} {Unit}"); }
        }

        public override string ToString()
        {
            return DisplayValue;
        }
    }
}
