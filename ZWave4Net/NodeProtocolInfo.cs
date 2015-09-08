using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net
{
    public class NodeProtocolInfo
    {
        private readonly byte _capability;
        private readonly byte _reserved;

        public readonly BasicType BasicType;
        public readonly GenericType GenericType;
        public readonly byte SpecificType;
        public readonly Security Security;

        public NodeProtocolInfo(byte[] data)
        {
            _capability = data[0];
            Security = (Security)data[1];
            _reserved = data[2];

            BasicType = (BasicType)data[3];
            GenericType = (GenericType)data[4];
            SpecificType = data[5];
        }

        public bool Routing
        {
            get { return (_capability & 0x40) != 0; }
        }

        public bool Listening
        {
            get { return (_capability & 0x80) != 0; }
        }

        public byte Version
        {
            get { return (byte)((_capability & 0x07) + 1); }
        }

        public int MaxBaudrate
        {
            get { return ((_capability & 0x38) == 0x10) ? 40000 : 9600; }
        }

        public override string ToString()
        {
            return string.Format($"GenericType = {GenericType}, BasicType = {BasicType}, Listening = {Listening}, Version = {Version}, Security = {Security}, Routing = {Routing}, MaxBaudrate = {MaxBaudrate}");
        }
    }
}
