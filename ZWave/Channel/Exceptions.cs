using System;

namespace ZWave.Channel
{
    public class CommunicationException : Exception
    {
        public CommunicationException() : base("Communication error") { }
        public CommunicationException(string message) : base(message) { }
        public CommunicationException(string message, Exception inner) : base(message, inner) { }
    }

    public class VesrionNotSupportedException : Exception
    {
        public VesrionNotSupportedException() : base("version not supported") { }
        public VesrionNotSupportedException(string message) : base(message) { }
        public VesrionNotSupportedException(string message, Exception inner) : base(message, inner) { }
    }
}
