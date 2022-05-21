using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Channel.Protocol
{
    class NodeCommand : Message
    {
        private static byte callbackID = 0;
        public readonly byte NodeID;
        public readonly Command Command;
        public readonly byte CallbackID;

        public NodeCommand(byte nodeID, Command command)
            : base(FrameHeader.SOF, MessageType.Request, Channel.Function.SendData)
        {
            if ((NodeID = nodeID) == 0)
                throw new ArgumentOutOfRangeException(nameof(NodeID), nodeID, "NodeID can not be 0");
            if ((Command = command) == null)
                throw new ArgumentNullException(nameof(command));

            CallbackID = GetNextCallbackID();
        }

        public override string ToString()
        {
            return string.Concat(base.ToString(), " ", $"NodeID:{NodeID:D3}, Command:[{Command}], CallbackID:{CallbackID}");
        }

        private static byte GetNextCallbackID()
        {
            lock (typeof(Message)) { return callbackID = (byte)((callbackID % 255) + 1); }
        }

        protected override List<byte> GetPayload()
        {
            var payload = base.GetPayload();
            payload.Add(NodeID);
            payload.AddRange(Command.ToBytes());
            payload.Add((byte)(TransmitOptions.Ack | TransmitOptions.AutoRoute | TransmitOptions.Explore));
            payload.Add(CallbackID);
            return payload;
        }
    }
}
