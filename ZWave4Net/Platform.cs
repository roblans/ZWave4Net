using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net
{
    public static class Platform
    {
        public static Action<LogLevel, string> Log = (_, __) => { };
    }
}
