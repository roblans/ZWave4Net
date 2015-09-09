using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net.Communication
{
    public class CommunicationException : Exception
    {
        public CommunicationException() { }
        public CommunicationException(string message) : base(message) { }
        public CommunicationException(string message, Exception inner) : base(message, inner) { }
    }

    public class ChecksumException : CommunicationException
    {
        public ChecksumException() { }
        public ChecksumException(string message) : base(message) { }
        public ChecksumException(string message, Exception inner) : base(message, inner) { }
    }

    public class UnknownFrameException : CommunicationException
    {
        public UnknownFrameException() { }
        public UnknownFrameException(string message) : base(message) { }
        public UnknownFrameException(string message, Exception inner) : base(message, inner) { }
    }

    public class NakResponseException : CommunicationException
    {
        public NakResponseException() { }
        public NakResponseException(string message) : base(message) { }
        public NakResponseException(string message, Exception inner) : base(message, inner) { }
    }
}
