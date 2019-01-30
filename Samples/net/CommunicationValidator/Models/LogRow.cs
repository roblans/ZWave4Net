using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationValidator.Models
{
    public class LogRow
    {
        public LogRow(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            if (!bytes.Any()) throw new ArgumentException(nameof(bytes));

            Message = string.Join(", ", BitConverter.ToString(bytes).Split('-').Select(v => $"0x{v}"));
            MessageDescription = GetMessageDescription(bytes);

            IsAck = bytes.First() == 0x06;
            IsNakOrCan = bytes.First() == 0x15 || bytes.First() == 0x18;

            Mode = CommunicationMode.Received; //todo
        }

        public string Message { get; }

        public CommunicationMode Mode { get; }

        public bool IsAck { get; }
        public bool IsNakOrCan { get; }

        public string MessageDescription { get; }


        private static string GetMessageDescription(byte[] bytes)
        {
            if (!bytes.Any())
                return "Empty row";

            if (bytes.Count() == 1)
            {
                var b = bytes.First();
                if (b == 0x01)
                    return "SOF";
                if (b == 0x06)
                    return "ACK";
                if (b == 0x15)
                    return "NAK";
                if (b == 0x18)
                    return "CAN";

                return "Unknown";
            }
            else
            {
                // todo message
            }

            return null;
        }
    }

    public enum CommunicationMode
    {
        Unknown,

        Received,
        Send
    }
}
