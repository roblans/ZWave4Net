using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Driver.Communication
{
    public class CommunicationException : Exception
    {
        public CommunicationException() { }
        public CommunicationException(string message) : base(message) { }
        public CommunicationException(string message, Exception inner) : base(message, inner) { }
    }
}
