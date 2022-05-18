using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave
{
    public class ErrorEventArgs : EventArgs
    {
        public readonly Exception Error;

        public ErrorEventArgs(Exception error)
        {
            Error = error;
        }
    }
}
