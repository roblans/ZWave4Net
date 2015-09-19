using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave4Net.Commands;

namespace ZWave4Net.Communication
{
    class EventMessage
    {
        public ReceiveStatus Status { get; private set; }
        public byte NodeID { get; private set; }
        public Command Command { get; private set; }

        public static EventMessage Parse(byte[] data)
        {
            var eventMessage = new EventMessage();
            eventMessage.Status = (ReceiveStatus)data[0];
            eventMessage.NodeID = data[1];
            eventMessage.Command = Command.Parse(data.Skip(2).ToArray());
            return eventMessage;
        }
    }
}
