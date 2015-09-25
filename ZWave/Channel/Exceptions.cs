using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Channel
{
    public class CommunicationException : System.Exception
    {
        public CommunicationException() { }
        public CommunicationException(string message) : base(message) { }
        public CommunicationException(string message, System.Exception inner) : base(message, inner) { }
    }
}
