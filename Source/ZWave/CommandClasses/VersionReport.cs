using System;
using System.Collections.Generic;
using System.Text;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class VersionReport : NodeReport
    {
        public readonly LibraryType Library;
        public readonly string[] Firmware;
        public readonly string Protocol;
        public readonly byte Hardware;

        internal VersionReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 5)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            Library = (LibraryType)payload[0];
            Protocol = payload[1].ToString("d") + "." + payload[2].ToString("d2");
            List<string> firmwares = new List<string>();
            firmwares.Add(payload[3].ToString("d") + "." + payload[4].ToString("d2"));

            if (payload.Length > 6)
            {
                Hardware = payload[5];
                //Version 2+
                byte numFirmwares = payload[6];
                for (byte i = 0; i < numFirmwares; i++)
                {
                    firmwares.Add(payload[7 + (i * 2)].ToString("d") + "." + payload[8 + (i * 2)].ToString("d2"));
                }
            }
            else
                Hardware = 0;

            Firmware = firmwares.ToArray();
        }

        public override string ToString()
        {
            return $"Library:{Library}, Firmware:{string.Join(",", Firmware)}, Protocol:{Protocol},Hardware:{Hardware}";
        }
    }
}
