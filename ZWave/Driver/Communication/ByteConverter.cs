using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Driver.Communication
{
    static class ByteConverter
    {
        public static UInt32 ToUInt32(byte[] value, int startIndex = 0)
        {
            return BitConverter.ToUInt32(value.Skip(startIndex).Take(sizeof(UInt32)).Reverse().ToArray(), 0);
        }

        public static Int32 ToInt32(byte[] value, int startIndex = 0)
        {
            return BitConverter.ToInt32(value.Skip(startIndex).Take(sizeof(Int32)).Reverse().ToArray(), 0);
        }

        public static UInt64 ToUInt64(byte[] value, int startIndex = 0)
        {
            return BitConverter.ToUInt64(value.Skip(startIndex).Take(sizeof(UInt64)).Reverse().ToArray(), 0);
        }

        public static Int64 ToInt64(byte[] value, int startIndex = 0)
        {
            return BitConverter.ToInt64(value.Skip(startIndex).Take(sizeof(Int64)).Reverse().ToArray(), 0);
        }
    }
}
