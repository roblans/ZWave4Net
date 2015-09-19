using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net.Communication
{
    class MessageEventArgs : EventArgs
    {
        public readonly Message Message;

        public MessageEventArgs(Message message)
        {
            Message = message;
        }
    }
}
