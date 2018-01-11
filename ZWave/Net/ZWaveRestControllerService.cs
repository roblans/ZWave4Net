using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Net
{
    class ZWaveRestControllerService
    {
        public readonly ZWaveController Controller;

        public ZWaveRestControllerService(ZWaveController controller)
        {
            Controller = controller;
        }
    }
}
