using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net.Console
{
    class Program
    {
        public static LogLevel LogThreshold = LogLevel.Debug;

        static void Main(string[] args)
        {
            Platform.Log = Log;

            var port = new SerialPort(System.IO.Ports.SerialPort.GetPortNames().First());
            var driver = new ZWaveDriver(port);
            driver.Open().Wait();
            try
            {
                System.Console.ReadLine();
            }
            finally
            {
                driver.Close();
            }
        }

        private static void Log(LogLevel level, string message)
        {
            lock (typeof(Program))
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
