using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Channel.Protocol
{
    class NodeInformation : Message
    {
        public NodeInformation(byte[] payload)
            : base(FrameHeader.SOF, MessageType.Response, Channel.Function.ApplicationUpdate)
        {
        }
   }
}
