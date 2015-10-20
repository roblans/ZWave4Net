using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Channel.Protocol
{
    public class ProtocolException : CommunicationException
    {
        public ProtocolException() { }
        public ProtocolException(string message) : base(message) { }
        public ProtocolException(string message, Exception inner) : base(message, inner) { }
    }

    public class ChecksumException : ProtocolException
    {
        public ChecksumException() { }
        public ChecksumException(string message) : base(message) { }
        public ChecksumException(string message, Exception inner) : base(message, inner) { }
    }

    public class UnknownFrameException : ProtocolException
    {
        public UnknownFrameException() { }
        public UnknownFrameException(string message) : base(message) { }
        public UnknownFrameException(string message, Exception inner) : base(message, inner) { }
    }

    public class TransmissionException : ProtocolException
    {
        public TransmissionException() { }
        public TransmissionException(string message) : base(message) { }
        public TransmissionException(string message, System.Exception inner) : base(message, inner) { }
    }

    public class ResponseException : ProtocolException
    {
        public ResponseException() { }
        public ResponseException(string message) : base(message) { }
        public ResponseException(string message, Exception inner) : base(message, inner) { }
    }

    public class NakResponseException : ResponseException
    {
        public NakResponseException() { }
        public NakResponseException(string message) : base(message) { }
        public NakResponseException(string message, Exception inner) : base(message, inner) { }
    }

    public class CanResponseException : ResponseException
    {
        public CanResponseException() { }
        public CanResponseException(string message) : base(message) { }
        public CanResponseException(string message, Exception inner) : base(message, inner) { }
    }

    public class ReponseFormatException : ResponseException
    {
        public ReponseFormatException() { }
        public ReponseFormatException(string message) : base(message) { }
        public ReponseFormatException(string message, System.Exception inner) : base(message, inner) { }
    }
}
