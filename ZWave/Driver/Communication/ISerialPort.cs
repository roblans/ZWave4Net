using System.IO;

namespace ZWave.Driver.Communication
{
    public interface ISerialPort
    {
        Stream InputStream { get; }
        Stream OutputStream { get; }

        void Close();
        void Open();
    }
}