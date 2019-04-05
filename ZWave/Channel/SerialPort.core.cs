using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Channel
{
#if NETCOREAPP2_0 || NETSTANDARD2_0
    public class SerialPort : ISerialPort
    {
        private readonly RJCP.IO.Ports.SerialPortStream _serialPortStream;

        public SerialPort(string name)
        {
            _serialPortStream = new RJCP.IO.Ports.SerialPortStream(name, 115200, 8, RJCP.IO.Ports.Parity.None, RJCP.IO.Ports.StopBits.One);
        }

        public Stream InputStream
        {
            get { return _serialPortStream; }
        }

        public Stream OutputStream
        {
            get { return _serialPortStream; }
        }

        public void Open()
        {
            _serialPortStream.Open();
        }

        public void Close()
        {
            _serialPortStream.Close();
        }
    }
#endif
}
