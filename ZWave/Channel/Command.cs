using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Channel
{
    public class Command
    {
        private readonly object _class;
        private readonly object _command;
        public readonly byte[] Payload;

        private Command(object @class, object command, params byte[] payload)
        {
            _class = @class;
            _command = command;
            Payload = payload;
        }

        public Command(byte classID, byte commandID, params byte[] payload)
            : this((object)classID, (object)commandID, payload)
        {
        }

        public Command(CommandClass @class, byte commandID, params byte[] payload)
            : this((object)@class, (object)commandID, payload)
        {
        }

        public Command(byte classID, Enum command, params byte[] payload)
            : this((object)classID, (object)command, payload)
        {
        }

        public Command(CommandClass @class, Enum command, params byte[] payload)
            : this((object)@class, (object)command, payload)
        {

        }

        public byte ClassID
        {
            get { return Convert.ToByte(_class); }
        }

        public byte CommandID
        {
            get { return Convert.ToByte(_command); }
        }

        public override string ToString()
        {
            return $"Class:{_class:X}, Command:{_command}, Payload:{BitConverter.ToString(Payload)}";
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
