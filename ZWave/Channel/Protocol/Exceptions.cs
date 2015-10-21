using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Channel.Protocol
{
    public class ChecksumException : CommunicationException
    {
        public ChecksumException() : base("Invalid checksum received.") { }
        public ChecksumException(string message) : base(message) { }
        public ChecksumException(string message, Exception inner) : base(message, inner) { }
    }

    public class UnknownFrameException : CommunicationException
    {
        public UnknownFrameException() : base("Unknown frame received.") { }
        public UnknownFrameException(string message) : base(message) { }
        public UnknownFrameException(string message, Exception inner) : base(message, inner) { }
    }

    public class TransmissionException : CommunicationException
    {
        public TransmissionException() : base("Transmission failure.") { }
        public TransmissionException(string message) : base(message) { }
        public TransmissionException(string message, System.Exception inner) : base(message, inner) { }
    }

    public class ResponseException : CommunicationException
    {
        public ResponseException() : base("Invalid response received.") { }
        public ResponseException(string message) : base(message) { }
        public ResponseException(string message, Exception inner) : base(message, inner) { }
    }

    public class NakResponseException : ResponseException
    {
        public NakResponseException() : base("NAK response received.") { }
        public NakResponseException(string message) : base(message) { }
        public NakResponseException(string message, Exception inner) : base(message, inner) { }
    }

    public class CanResponseException : ResponseException
    {
        public CanResponseException() : base("CAN response received.") { }
        public CanResponseException(string message) : base(message) { }
        public CanResponseException(string message, Exception inner) : base(message, inner) { }
    }

    public class ReponseFormatException : ResponseException
    {
        public ReponseFormatException() : base("The response was not in the expected format") { }
        public ReponseFormatException(string message) : base(message) { }
        public ReponseFormatException(string message, System.Exception inner) : base(message, inner) { }
    }
}
