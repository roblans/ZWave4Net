using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Driver.Communication.Protocol
{
    class NodeCommandCompleted : Message
    {
        public readonly byte CallbackID;
        public readonly TransmissionState TransmissionState;
        public readonly byte UnknownByte1;
        public readonly byte UnknownByte2;

        public NodeCommandCompleted(byte[] payload) : 
            base(FrameHeader.SOF, MessageType.Request, Communication.Function.SendData)
        {
            CallbackID = payload[0];
            TransmissionState = (TransmissionState)payload[1];
            UnknownByte1 = payload[2];
            UnknownByte2 = payload[3];
        }

        public override string ToString()
        {
            return string.Concat(base.ToString(), " ", string.Format($"CallbackID:{CallbackID} {TransmissionState} {UnknownByte1}? {UnknownByte2}?"));
        }
    }
}
