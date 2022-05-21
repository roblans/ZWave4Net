using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Channel.Protocol
{
    class ControllerFunctionCompleted : ControllerFunctionMessage
    {
        public ControllerFunctionCompleted(Function function, byte[] payload)
            : base(MessageType.Response, function, payload)
        {
        }
    }
}
