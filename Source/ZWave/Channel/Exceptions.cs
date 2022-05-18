using System;

namespace ZWave.Channel
{
    public class CommunicationException : Exception
    {
        public CommunicationException() : base("Communication error") { }
        public CommunicationException(string message) : base(message) { }
        public CommunicationException(string message, Exception inner) : base(message, inner) { }
    }

    public class VersionNotSupportedException : Exception
    {
        public VersionNotSupportedException() : base("version not supported") { }
        public VersionNotSupportedException(string message) : base(message) { }
        public VersionNotSupportedException(string message, Exception inner) : base(message, inner) { }
    }
}
