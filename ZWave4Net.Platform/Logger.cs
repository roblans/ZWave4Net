using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave4Net
{
    public static class Logger
    {
        public static LogLevel LogThreshold = LogLevel.Info;

        public static void LogMessage(LogLevel level, string message)
        {
            lock (typeof(Logger))
            {
                if (level < LogThreshold)
                    return;

                switch (level)
                {
                    case LogLevel.Debug:
                        System.Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogLevel.Info:
                        System.Console.ForegroundColor = ConsoleColor.White;
                        break;
                    case LogLevel.Warn:
                        System.Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case LogLevel.Error:
                        System.Console.ForegroundColor = ConsoleColor.Red;
                        break;
                }
                System.Console.WriteLine("{0} {1}", DateTime.Now.ToString("HH:mm:ss"), message);
            };
        }
    }
}
