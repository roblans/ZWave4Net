using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZWave.Channel;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class ManufacturerSpecificDeviceReport : NodeReport
    {
        public readonly DeviceSpecificType Type;
        public readonly string ID;

        internal ManufacturerSpecificDeviceReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 2)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            Type = (DeviceSpecificType)(payload[0] & 0x07);
            bool binary = true;
            if ((payload[1] & 0xE0) == 0x0)
                binary = false;
            int len = payload[1] & 0x1F;
            if (payload.Length < len + 2)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");
            if (binary)
                ID = BitConverter.ToString(payload, 2, len);
            else
                ID = Encoding.UTF8.GetString(payload, 2, len);
        }

        public override string ToString()
        {
            return $"Type:{Type}, ID:{ID}";
        }
    }
}
