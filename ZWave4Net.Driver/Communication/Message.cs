using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net.Driver.Communication
{
    class Message
    {
        private static byte callbackID = 0;
        public static readonly Message Acknowledgment = new Message(FrameHeader.ACK);
        public static readonly Message NegativeAcknowledgment = new Message(FrameHeader.NAK);
        public static readonly Message Cancel = new Message(FrameHeader.CAN);

        public readonly FrameHeader Header;
        public readonly MessageType? Type;
        public readonly Function? Function;
        public readonly byte[] Payload;

        private Message(FrameHeader header, MessageType? type = null, Function? function = null, byte[] payload = null)
        {
            Header = header;
            Type = type;
            Function = function;
            Payload = payload;
        }

        public Message(MessageType type, Function function, params byte[] payload)
            : this(FrameHeader.SOF, type, function, payload)
        {
        }

        private static byte GetNextCallbackID()
        {
            lock (typeof(Message)) { return callbackID = (byte)((callbackID % 255) + 1); }
        }

        public override string ToString()
        {
            if (Header == FrameHeader.SOF)
            {
                var payload = Payload != null ? BitConverter.ToString(Payload) : null;
                return string.Format($"{Header}, {Type}, {Function}, {payload}");
            }
            return string.Format($"{Header}");
        }

        public Task Write(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (Header == FrameHeader.SOF)
            {
                var buffer = new List<byte>();
                buffer.Add((byte)FrameHeader.SOF);

                // placeholder for length, update below
                buffer.Add(0x00);

                buffer.Add((byte)Type);
                buffer.Add((byte)Function);

                if (Payload != null)
                {
                    buffer.AddRange(Payload);
                }

                if (Function == Communication.Function.SendData)
                {
                    buffer.Add((byte)(TransmitOptions.Ack | TransmitOptions.AutoRoute | TransmitOptions.Explore));
                    buffer.Add(GetNextCallbackID());
                }

                // update length
                buffer[1] = (byte)(buffer.Count - 1);

                // add checksum 
                buffer.Add(buffer.Skip(1).Aggregate((byte)0xFF, (total, next) => total ^= next));

                Platform.LogMessage(LogLevel.Debug, string.Format($"Transmitted: {BitConverter.ToString(buffer.ToArray())}"));

                return stream.WriteAsync(buffer.ToArray(), 0, buffer.Count);
            }

            switch (Header)
            {
                case FrameHeader.ACK:
                    return stream.WriteAsync(new[] { (byte)FrameHeader.ACK }, 0, 1);
                case FrameHeader.NAK:
                    return stream.WriteAsync(new[] { (byte)FrameHeader.NAK }, 0, 1);
                case FrameHeader.CAN:
                    return stream.WriteAsync(new[] { (byte)FrameHeader.CAN }, 0, 1);
            }

            throw new NotSupportedException("Frameheader is not supported");
        }

        public async static Task<Message> Read(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var buffer = new byte[1024];

            await stream.ReadAsyncExact(buffer, 0, 1);
            var header = (FrameHeader)buffer[0];

            switch (header)
            {
                case FrameHeader.ACK:
                    Platform.LogMessage(LogLevel.Debug, string.Format($"Received: {buffer[0]}"));
                    return Message.Acknowledgment;
                case FrameHeader.NAK:
                    Platform.LogMessage(LogLevel.Debug, string.Format($"Received: {buffer[0]}"));
                    return Message.NegativeAcknowledgment;
                case FrameHeader.CAN:
                    Platform.LogMessage(LogLevel.Debug, string.Format($"Received: {buffer[0]}"));
                    return Message.Cancel;
            }

            if (header == FrameHeader.SOF)
            {
                await stream.ReadAsyncExact(buffer, 1, 1);
                var length = buffer[1];

                buffer = buffer.Take(length + 2).ToArray();
                await stream.ReadAsyncExact(buffer, 2, length);

                var type = (MessageType)buffer[2];
                var function = (Function)buffer[3];
                var payload = buffer.Skip(4).Take(length - 3).ToArray();

                Platform.LogMessage(LogLevel.Debug, string.Format($"Received: {BitConverter.ToString(buffer.ToArray())}"));

                if (buffer.Skip(1).Take(buffer.Length - 2).Aggregate((byte)0xFF, (total, next) => (byte)(total ^ next)) != buffer[buffer.Length - 1])
                    throw new ChecksumException();

                return new Message(type, function, payload);
            }

            throw new UnknownFrameException("Frameheader is not supported");
        }
    }

    static partial class Extensions
    {
        public static async Task ReadAsyncExact(this Stream stream, byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            var read = 0;
            while (read < count)
            {
                read += await stream.ReadAsync(buffer, offset + read, count - read);
            }
        }
    }
}
