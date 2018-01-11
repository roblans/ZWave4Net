#if NET

using System;
using System.IO;
using System.ServiceModel.Web;

namespace ZWave.Net
{
    public class ZWaveRestServer : IDisposable
    {
        private readonly WebServiceHost _host;
        public readonly ZWaveController Controller;
        public readonly Uri Address;
        public TextWriter Log { get; set; }

        public ZWaveRestServer(ZWaveController controller, int port = 80)
        {
            Address = new Uri($"http://127.0.0.1:{port}/api/v1.0/");
            _host = new WebServiceHost(new ControllerRestService(controller), Address);
        }

        private void LogMessage(string message)
        {
            if (Log != null && message != null)
            {
                Log.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd H:mm:ss.fff")} {message}");
            }
        }

        public void Open()
        {
            LogMessage($"Opening ZWaveRestServer, baseaddress: {Address}");

            _host.Open();
        }

        public void Close()
        {
            LogMessage("Closing ZWaveRestServer");

            _host.Close();
        }

        public void Dispose()
        {
            using (_host) { };
        }
    }
}

#endif
