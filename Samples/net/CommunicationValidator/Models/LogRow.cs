using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationValidator.Models
{
    public class LogRow
    {
        public LogRow(string line)
        {
            DateTime = DateTime.ParseExact(line.Remove(23), "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            Mode = line.Remove(0, 24).StartsWith("Received:") ? CommunicationMode.Received : CommunicationMode.Send;

            var message = line.Remove(0, 24).Split(new[] { ':' }, 2).Last().Trim();
            if (message == "ACK")
            {
                Message = message;
                MessageDescription = message;
                IsAck = true;
            }
            else if (message == "NAK")
            {
                Message = message;
                MessageDescription = message;
                IsNakOrCan = true;
            }
            else if (message == "CAN")
            {
                Message = message;
                MessageDescription = message;
                IsNakOrCan = true;
            }
            else
            {
                var messageParts = message.Split(',').Select(el => el.Trim());
                var nodePart = messageParts.FirstOrDefault(el => el.StartsWith("NodeID:"));
                if (!string.IsNullOrEmpty(nodePart))
                    NodeID = byte.Parse(nodePart.Remove(0, 7));
                var commandPart = messageParts.FirstOrDefault(el => el.StartsWith("Command:") && !el.StartsWith("Command:["));
                if (!string.IsNullOrEmpty(commandPart))
                    Command = commandPart.Remove(0, 8);

                MessageDescription = message;
            }
        }

        public byte? NodeID { get; }
        public string Command { get; }

        public DateTime DateTime { get; }

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
