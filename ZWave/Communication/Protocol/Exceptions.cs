using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Communication.Protocol
{
    public class ProtocolException : CommunicationException
    {
        public ProtocolException() { }
        public ProtocolException(string message) : base(message) { }
        public ProtocolException(string message, System.Exception inner) : base(message, inner) { }
    }

    public class ChecksumException : ProtocolException
    {
        public ChecksumException() { }
        public ChecksumException(string message) : base(message) { }
        public ChecksumException(string message, System.Exception inner) : base(message, inner) { }
    }

    public class NakResponseException : ProtocolException
    {
        public NakResponseException() { }
        public NakResponseException(string message) : base(message) { }
        public NakResponseException(string message, System.Exception inner) : base(message, inner) { }
    }

    public class CanResponseException : ProtocolException
    {
        public CanResponseException() { }
        public CanResponseException(string message) : base(message) { }
        public CanResponseException(string message, System.Exception inner) : base(message, inner) { }
    }

    public class UnknownFrameException : ProtocolException
    {
        public UnknownFrameException() { }
        public UnknownFrameException(string message) : base(message) { }
        public UnknownFrameException(string message, System.Exception inner) : base(message, inner) { }
    }
}
