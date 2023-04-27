using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave
{
    public class NodeProtocolInfo
    {
        public byte Capability { get; private set; }
        public byte Reserved { get; private set; }
        public BasicType BasicType { get; private set; }
        public GenericType GenericType { get; private set; }
        public SpecificType SpecificType { get; private set; }
        public Security Security { get; private set; }

        public static NodeProtocolInfo Parse(byte[] data)
        {
            return new NodeProtocolInfo()
            {
                Capability = data[0],
                Security = (Security)data[1],
                Reserved = data[2],
                BasicType = (BasicType)data[3],
                GenericType = (GenericType)data[4],
                SpecificType = SpecificTypeMapping.Get((GenericType)data[4], data[5]),
            };
        }

        public bool Routing
        {
            get { return (Capability & 0x40) != 0; }
        }

        public bool IsListening
        {
            get { return (Capability & 0x80) != 0; }
        }

        public byte Version
        {
            get { return (byte)((Capability & 0x07) + 1); }
        }

        public int MaxBaudrate
        {
            get { return ((Capability & 0x38) == 0x10) ? 40000 : 9600; }
        }

        public override string ToString()
        {
            return $"GenericType = {GenericType}, BasicType = {BasicType}, Listening = {IsListening}, Version = {Version}, Security = [{Security}], Routing = {Routing}, MaxBaudrate = {MaxBaudrate}";
        }
    }
}
