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

    public class ProtocolException : CommunicationException
    {
        public ProtocolException() { }
        public ProtocolException(string message) : base(message) { }
        public ProtocolException(string message, Exception inner) : base(message, inner) { }
    }

    public class ResponseException : CommunicationException
    {
        public ResponseException() { }
        public ResponseException(string message) : base(message) { }
        public ResponseException(string message, Exception inner) : base(message, inner) { }
    }

    public class ChecksumException : ResponseException
    {
        public ChecksumException() { }
        public ChecksumException(string message) : base(message) { }
        public ChecksumException(string message, Exception inner) : base(message, inner) { }
    }

    public class NakResponseException : ResponseException
    {
        public NakResponseException() { }
        public NakResponseException(string message) : base(message) { }
        public NakResponseException(string message, Exception inner) : base(message, inner) { }
    }
}
