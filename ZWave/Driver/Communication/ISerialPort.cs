using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Driver.Communication
{
    public interface ISerialPort
    {
        void Open();
        void Close();
        Stream InputStream { get; }
        Stream OutputStream { get; }
    }
}
