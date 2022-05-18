using System;
using System.IO;
using System.Linq;

namespace ZWave.Channel
{
#if NET6_0 || NET5_0 || NET48
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

#elif WINDOWS_UWP

    using Windows.Devices.SerialCommunication;
    using Windows.Storage.Streams;
    using System.Runtime.InteropServices.WindowsRuntime;

    public class SerialPort : ISerialPort
    {
        private readonly string _id;
        private SerialDevice _device;
        private Stream _inputStream;
        private Stream _outputStream;

        public Stream InputStream
        {
            get { return _inputStream; }
        }

        public Stream OutputStream
        {
            get { return _outputStream; }
        }

        public object Parity { get; private set; }

        public SerialPort(string name)
        {
            var selector = SerialDevice.GetDeviceSelector(name);
            var devices = Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(selector, null).AsTask().Result;
            if (!devices.Any())
                throw new ArgumentOutOfRangeException(nameof(name), name, "Serialport not found");

            _id = devices.First().Id;
        }

        public SerialPort(ushort vendorId, ushort productId)
        {
            var selector = SerialDevice.GetDeviceSelectorFromUsbVidPid(vendorId, productId);
            var devices = Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(selector, null).AsTask().Result;
            if (!devices.Any())
                throw new ArgumentOutOfRangeException("Serialport not found, invalid vendorId or productId");

            _id = devices.First().Id;
        }

        public void Open()
        {
            _device = SerialDevice.FromIdAsync(_id).AsTask().Result;
            if (_device == null)
                throw new Exception("Error opening serialdevice, make sure the app manifest contains a DeviceCapability section for the serialport.");

            _device.BaudRate = 115200;
            _device.Parity = SerialParity.None;
            _device.DataBits = 8;
            _device.StopBits = SerialStopBitCount.One;
            _inputStream = new SerialReadStream(_device.InputStream);
            _outputStream = new SerialWriteStream(_device.OutputStream);
        }

        public void Close()
        {
            if (_device != null)
            {
                _inputStream = null;
                _outputStream = null;
                _device.Dispose();
                _device = null;
            }
        }

        class SerialReadStream : Stream
        {
            private readonly IInputStream _input;

            public SerialReadStream(IInputStream input)
            {
                _input = input;
            }

            public override bool CanRead
            {
                get { return true; }
            }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanWrite
            {
                get { return false; }
            }

            public override long Length
            {
                get { throw new NotSupportedException(); }
            }

            public override long Position
            {
                get { throw new NotSupportedException(); }
                set { throw new NotSupportedException(); }
            }

            public override void Flush()
            {
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                try
                {
                    var bytes = new byte[1024];
                    _input.ReadAsync(bytes.AsBuffer(), (uint)count, InputStreamOptions.None).AsTask().Wait();
                    bytes.CopyTo(0, buffer.AsBuffer(), (uint)offset, count);
                    return count;
                }
                catch (Exception ex)
                {
                    throw new IOException("Read failed", ex);
                }
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotImplementedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }
        }


        class SerialWriteStream : Stream
        {
            private readonly IOutputStream _output;

            public SerialWriteStream(IOutputStream output)
            {
                _output = output;
            }

            public override bool CanRead
            {
                get { return false; }
            }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanWrite
            {
                get { return true; }
            }

            public override long Length
            {
                get { throw new NotSupportedException(); }
            }

            public override long Position
            {
                get { throw new NotSupportedException(); }
                set { throw new NotSupportedException(); }
            }

            public override void Flush()
            {
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotImplementedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                try
                {
                    _output.WriteAsync(buffer.Skip(offset).Take(count).ToArray().AsBuffer()).AsTask().Wait();
                }
                catch (Exception ex)
                {
                    throw new IOException("Write failed", ex);
                }
            }
        }
    }

#else

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
