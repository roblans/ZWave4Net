using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Driver.Communication
{
    public class Command
    {
        public enum Class : byte
        {
            Basic = 0x20,
            SwitchBinary = 0x25,
        }

        public readonly byte ClassID;
        public readonly byte CommandID;
        public readonly byte[] Payload;

        public Command(byte classID, byte commandID, params byte[] payload)
        {
            ClassID = classID;
            CommandID = commandID;
            Payload = payload;
        }
        public Command(Class @class, Enum command, params byte[] payload)
            : this(Convert.ToByte(@class), Convert.ToByte(command), payload)
        {
        }
        public Command(Class @class, byte commandID, params byte[] payload)
            : this(Convert.ToByte(@class), commandID, payload)
        {
        }

        public Command(byte classID, Enum command, params byte[] payload)
            : this(classID, Convert.ToByte(command), payload)
        {
        }

        public override string ToString()
        {
            var className = Enum.IsDefined(typeof(Class), ClassID) ? Enum.ToObject(typeof(Class), ClassID).ToString() : ClassID.ToString();
            return string.Format($"Class:{className} CommandID:{CommandID} Payload:{BitConverter.ToString(Payload)}");
        }

        public byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.Add(0);
            bytes.Add(ClassID);
            bytes.Add(CommandID);
            bytes.AddRange(Payload);
            bytes[0] = (byte)(bytes.Count - 1);
            return bytes.ToArray();
        }

        public static Command Parse(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var length = data[0];
            var classID = data[1];
            var commandID = data[2];
            var payload = data.Skip(3).Take(length - 2).ToArray();
            return new Command(classID, commandID, payload);
        }
    }
}
