using System.IO;

#if WINDOWS_UWP
using Windows.Devices.SerialCommunication;
#endif

namespace ZWave.Channel
{
#if WINDOWS_UWP
    public class SerialPort : ISerialPort
    {
        private readonly SerialDevice  _device;

        public Stream InputStream
        {
            get { return _device.InputStream.AsStreamForRead(); }
        }

        public Stream OutputStream
        {
            get { return _device.OutputStream.AsStreamForWrite(); }
        }

        public object Parity { get; private set; }

        public SerialPort(string name)
        {
            _device = SerialDevice.FromIdAsync(name).GetResults();
            _device.BaudRate = 115200;
            _device.Parity = SerialParity.None;
            _device.DataBits = 8;
            _device.StopBits = SerialStopBitCount.One;
        }

        public void Open()
        {
        }

        public void Close()
        {
            _device.Dispose();
        }
    }
#endif
}