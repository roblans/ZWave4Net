using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Channel.Protocol
{
    class ControllerFunctionEvent : ControllerFunctionMessage
    {
        public ControllerFunctionEvent(Function function, byte[] payload)
            : base(MessageType.Request, function, payload)
        {
        }
    }
}
