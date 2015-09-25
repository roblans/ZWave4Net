using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Controller
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

        public static float ToSensorValue(byte[] payload, out byte scale)
        {
            // http://www.google.nl/url?q=http://www.cooperindustries.com/content/dam/public/wiringdevices/products/documents/technical_specifications/aspirerf_adtechguide_100713.pdf&sa=U&ved=0CBwQFjAAOApqFQoTCN-G7K7djcgCFeZr2wod5eILEA&usg=AFQjCNGzaMFiMosWoKLG-Wvo1A2p5QDbTw
            // http://www.google.nl/url?q=http://svn.linuxmce.org/trac/browser/trunk/src/ZWave/ZWApi.cpp%3Frev%3D27702&sa=U&ved=0CCYQFjAEOBRqFQoTCKboxvHkjcgCFYkH2wodxRoB0w&usg=AFQjCNFg3uMoEuAIX6R61kDS-5Q9C-F-GA
            // https://searchcode.com/codesearch/view/90040075/
            // bits 7,6,5: precision, bits 4,3: scale, bits 2,1,0 : size
            var precision = (byte)((payload[0] & 0xE0) >> 5);
            scale = (byte)((payload[0] & 0x18) >> 3);
            var size = (byte)(payload[0] & 0x07);

            var values = payload.Skip(1).ToArray();
            var bytes = Enumerable.Repeat((byte)((values[0] & 0x80) == 0x80 ? 0xFF : 0x00), sizeof(ulong) - size).Concat(values).ToArray();
            var value = ToInt64(bytes, 0);

            return (float)(value / Math.Pow(10, precision));
        }
    }
}
