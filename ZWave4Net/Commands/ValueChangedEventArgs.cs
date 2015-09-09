using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net.Commands
{
    public class ValueChangedEventArgs<T> : EventArgs
    {
        public readonly T Value;

        public ValueChangedEventArgs(T value)
        {
            Value = value;     
        }
    }
}
