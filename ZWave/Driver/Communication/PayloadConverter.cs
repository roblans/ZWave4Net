using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Driver.Communication
{
    public static class PayloadConverter
    {
        public static ushort ToUInt16(byte[] value, int startIndex = 0)
        {
            return BitConverter.ToUInt16(value.Skip(startIndex).Take(sizeof(UInt16)).Reverse().ToArray(), 0);
        }

        public static short ToInt16(byte[] value, int startIndex = 0)
        {
            return BitConverter.ToInt16(value.Skip(startIndex).Take(sizeof(Int16)).Reverse().ToArray(), 0);
        }

        public static uint ToUInt32(byte[] value, int startIndex = 0)
        {
            return BitConverter.ToUInt32(value.Skip(startIndex).Take(sizeof(UInt32)).Reverse().ToArray(), 0);
        }

        public static int ToInt32(byte[] value, int startIndex = 0)
        {
            return BitConverter.ToInt32(value.Skip(startIndex).Take(sizeof(Int32)).Reverse().ToArray(), 0);
        }

        public static ulong ToUInt64(byte[] value, int startIndex = 0)
        {
            return BitConverter.ToUInt64(value.Skip(startIndex).Take(sizeof(UInt64)).Reverse().ToArray(), 0);
        }

        public static long ToInt64(byte[] value, int startIndex = 0)
        {
            return BitConverter.ToInt64(value.Skip(startIndex).Take(sizeof(Int64)).Reverse().ToArray(), 0);
        }

        public static byte[] GetBytes(uint value)
        {
            return BitConverter.GetBytes(value).Reverse().ToArray();
        }
    }
}
