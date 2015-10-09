using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Channel.Protocol
{
    class NodeCommandCompleted : Message
    {
        public readonly byte CallbackID;
        public readonly TransmissionState TransmissionState;

        public NodeCommandCompleted(byte[] payload) : 
            base(FrameHeader.SOF, MessageType.Request, Channel.Function.SendData)
        {
            CallbackID = payload[0];
            TransmissionState = (TransmissionState)payload[1];
        }

        public override string ToString()
        {
            return string.Concat(base.ToString(), " ", $"CallbackID:{CallbackID}, {TransmissionState}");
        }
    }
}
