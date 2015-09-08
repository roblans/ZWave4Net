using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net.Communication
{
    class EventMessageEventArgs : EventArgs
    {
        public readonly EventMessage Message;

        public EventMessageEventArgs(EventMessage message)
        {
            Message = message;
        }
    }
}
