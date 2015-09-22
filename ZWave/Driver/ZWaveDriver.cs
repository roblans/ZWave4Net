using System;
using System.Collections.Generic;
using System.Text;
using ZWave.Driver.Communication;

namespace ZWave.Driver
{
    public class ZWaveDriver
    {
        public readonly ZWaveChannel Channel;

        public ZWaveDriver(ISerialPort port)
        {
            Channel = new ZWaveChannel(port);
        }

#if NET || WINDOWS_UWP
        public ZWaveDriver(string portName)
        {
            Channel = new ZWaveChannel(portName);
        }
#endif

        public void Open()
        {
            Channel.Open();
        }

        public void Close()
        {
            Channel.Close();
        }
    }
}
