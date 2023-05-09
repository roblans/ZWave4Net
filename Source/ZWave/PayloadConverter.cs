using System;
using System.Linq;

namespace ZWave
{
    public static class PayloadConverter
    {
        public static byte ToUInt8(byte[] value, int startIndex = 0)
        {
            return value.Skip(startIndex).First();
        }

        public static sbyte ToInt8(byte[] value, int startIndex = 0)
        {
            return unchecked((sbyte)value.Skip(startIndex).First());
        }

        public static ushort ToUInt16(byte[] value, int startIndex = 0)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.ToUInt16(value.Skip(startIndex).Take(sizeof(UInt16)).Reverse().ToArray(), 0);
            }
            else
            {
                return BitConverter.ToUInt16(value, startIndex);
            }
        }

        public static short ToInt16(byte[] value, int startIndex = 0)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.ToInt16(value.Skip(startIndex).Take(sizeof(Int16)).Reverse().ToArray(), 0);
            }
            else
            {
                return BitConverter.ToInt16(value, startIndex);
            }
        }

        public static uint ToUInt32(byte[] value, int startIndex = 0)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.ToUInt32(value.Skip(startIndex).Take(sizeof(UInt32)).Reverse().ToArray(), 0);
            }
            else
            {
                return BitConverter.ToUInt32(value, startIndex);
            }
        }

        public static int ToInt32(byte[] value, int startIndex = 0)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.ToInt32(value.Skip(startIndex).Take(sizeof(Int32)).Reverse().ToArray(), 0);
            }
            else
            {
                return BitConverter.ToInt32(value, startIndex);
            }
        }

        public static ulong ToUInt64(byte[] value, int startIndex = 0)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.ToUInt64(value.Skip(startIndex).Take(sizeof(UInt64)).Reverse().ToArray(), 0);
            }
            else
            {
                return BitConverter.ToUInt64(value, startIndex);
            }
        }

        public static long ToInt64(byte[] value, int startIndex = 0)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.ToInt64(value.Skip(startIndex).Take(sizeof(Int64)).Reverse().ToArray(), 0);
            }
            else
            {
                return BitConverter.ToInt64(value, startIndex);
            }
        }

        public static TimeSpan ToTimeSpan(byte payload)
        {
            if (payload == 0xFE || payload == 0x0)
                return TimeSpan.Zero;
            if (payload < 0x80)
                return new TimeSpan(0, 0, payload);
            else
                return new TimeSpan(0, payload - 0x80, 0);
        }

        public static byte GetByte(TimeSpan value)
        {
            if (value.TotalSeconds >= 1)
            {
                if (value.TotalSeconds < 127)
                    return (byte)value.TotalSeconds;
                else if (value.TotalMinutes < 126)
                    return (byte)(0x80 + value.TotalMinutes);
                else
                    return 0xFF;
            }
            return 0;
        }

        public static byte[] GetBytes(sbyte value)
        {
            return new byte[] { unchecked((byte)value) };
        }

        public static byte[] GetBytes(byte value)
        {
            return new byte[] { value };
        }

        public static byte[] GetBytes(short value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.GetBytes(value).Reverse().ToArray();
            }
            else
            {
                return BitConverter.GetBytes(value);
            }
        }

        public static byte[] GetBytes(ushort value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.GetBytes(value).Reverse().ToArray();
            }
            else
            {
                return BitConverter.GetBytes(value);
            }
        }

        public static byte[] GetBytes(int value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.GetBytes(value).Reverse().ToArray();
            }
            else
            {
                return BitConverter.GetBytes(value);
            }
        }

        public static byte[] GetBytes(uint value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.GetBytes(value).Reverse().ToArray();
            }
            else
            {
                return BitConverter.GetBytes(value);
            }
        }

        public static byte[] GetBytes(long value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.GetBytes(value).Reverse().ToArray();
            }
            else
            {
                return BitConverter.GetBytes(value);
            }
        }

        public static byte[] GetBytes(ulong value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.GetBytes(value).Reverse().ToArray();
            }
            else
            {
                return BitConverter.GetBytes(value);
            }
        }

        public static byte[] GetBytes(float value, byte decimals, byte scale)
        {
            var size = 0;
            var bytes = default(byte[]);
            var number = (long)Math.Round(value * Math.Pow(10, decimals));

            if (number >= sbyte.MinValue && number <= sbyte.MaxValue)
            {
                size = sizeof(sbyte);
                bytes = GetBytes((sbyte)number);
            }
            else if (number >= short.MinValue && number <= short.MaxValue)
            {
                size = sizeof(short);
                bytes = GetBytes((short)number);
            }
            else if (number >= int.MinValue && number <= int.MaxValue)
            {
                size = sizeof(int);
                bytes = GetBytes((int)number);
            }
            else
            {
                for (var i = decimals; i >= 0; i--)
                {
                    number = (long)Math.Round(value * Math.Pow(10, i));
                    if (number >= int.MinValue && number <= int.MaxValue)
                    {
                        decimals = i;
                        size = sizeof(int);
                        bytes = GetBytes((int)number);
                        break;
                    }
                }
            }

            if (bytes == null)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Value can not be converted in a valid format.");

            // create payload, patch first byte below
            var payload = new byte[] { 0x00 }.Concat(bytes).ToArray();

            // bits 7,6,5: precision, bits 4,3: scale, bits 2,1,0 : size
            payload[0] |= (byte)((decimals & 0x0f) << 5);
            payload[0] |= (byte)((scale & 0x03) << 3);
            payload[0] |= (byte)((size & 0x0f));

            return payload;
        }

        public static float ToFloat(byte[] payload, out byte scale)
        {
            // bits 7,6,5: precision, bits 4,3: scale, bits 2,1,0 : size
            var precision = (byte)((payload[0] & 0xE0) >> 5);
            scale = (byte)((payload[0] & 0x18) >> 3);
            var size = (byte)(payload[0] & 0x07);

            switch (size)
            {
                case sizeof(sbyte):
                    {
                        var value = (sbyte)payload[1];
                        return (float)(value / Math.Pow(10, precision));
                    }
                case sizeof(short):
                    {
                        var value = ToInt16(payload, 1);
                        return (float)(value / Math.Pow(10, precision));
                    }
                case sizeof(int):
                    {
                        var value = ToInt32(payload, 1);
                        return (float)(value / Math.Pow(10, precision));
                    }
            }
            return 0;
        }
    }
}
