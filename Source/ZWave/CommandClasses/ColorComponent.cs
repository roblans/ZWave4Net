using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.CommandClasses
{
    public class ColorComponent
    {
        public readonly byte ID;
        public readonly byte Value;

        public ColorComponent(byte id, byte value)
        {
            ID = id;
            Value = value;
        }

        public override string ToString()
        {
            return $"ID:{ID}, Value:{Value}";
        }

        public byte[] ToBytes()
        {
            return new[] { ID, Value };
        }
    }
}
