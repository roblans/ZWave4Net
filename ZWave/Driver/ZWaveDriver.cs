using System;
using System.Collections.Generic;
using System.Text;
using ZWave.Driver.Communication;

namespace ZWave.Driver
{
    public class ZWaveDriver
    {
        public readonly ZWaveChannel Channel;
        public readonly Controller Controller;

        private ZWaveDriver(ZWaveChannel channel)
        {
            Channel = channel;
            Controller = new Controller(this);
        }

        public ZWaveDriver(ISerialPort port)
            : this(new ZWaveChannel(port))
        {
        }

#if NET || WINDOWS_UWP
        public ZWaveDriver(string portName)
            : this(new ZWaveChannel(portName))
        {
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
