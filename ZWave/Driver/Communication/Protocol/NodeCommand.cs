using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Driver.Communication.Protocol
{
    class NodeCommand : Message
    {
        private static byte callbackID = 0;
        public readonly byte NodeID;
        public readonly Command Command;
        public readonly byte CallbackID;

        public NodeCommand(byte nodeID, Command command)
            : base(FrameHeader.SOF, MessageType.Request, Communication.Function.SendData)
        {
            NodeID = nodeID;
            Command = command;
            CallbackID = GetNextCallbackID();
        }

        public override string ToString()
        {
            return string.Concat(base.ToString(), " ", string.Format($"NodeID:{NodeID} Command:[{Command}] CallbackID:{CallbackID}"));
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
            payload.Add((byte)(TransmitOptions.Ack | TransmitOptions.AutoRoute | TransmitOptions.ForceRoute));
            payload.Add(CallbackID);
            return payload;
        }
    }
}
