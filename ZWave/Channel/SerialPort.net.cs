using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Channel
{
#if NET
    public class SerialPort : ISerialPort
    {
        private readonly System.IO.Ports.SerialPort _port;

        public Stream InputStream
        {
            get { return _port.BaseStream; }
        }

        public Stream OutputStream
        {
            get { return _port.BaseStream; }
        }

        public SerialPort(string name)
        {
            _port = new System.IO.Ports.SerialPort(name, 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
        }

        public void Open()
        {
            _port.Open();
            _port.DiscardInBuffer();
            _port.DiscardOutBuffer();
        }

        public void Close()
        {
            _port.Close();
        }
    }
#endif
}
