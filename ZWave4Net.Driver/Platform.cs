using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net.Driver
{
    public static class Platform
    {
        public static Action<LogLevel, string> LogMessage = (_, __) => { };
    }
}
