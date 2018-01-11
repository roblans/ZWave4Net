#if NET

using System;
using System.ServiceModel.Web;

namespace ZWave.Net
{
    public class ZWaveRestServer : IDisposable
    {
        private readonly WebServiceHost _host;
        public readonly ZWaveController Controller;
        public readonly Uri Address;

        public ZWaveRestServer(ZWaveController controller, int port)
        {
            Address = new Uri(string.Format("http://127.0.0.1:{0}/api/v1.0/", port));
            _host = new WebServiceHost(new ControllerService(controller), Address);
        }

        public void Open()
        {
            _host.Open();
        }

        public void Close()
        {
            _host.Close();
        }

        public void Dispose()
        {
            using (_host) { };
        }
    }
}

#endif
