using System;
using System.Collections.Generic;
using System.Text;
using ZWave.Channel.Protocol;
using ZWave.CommandClasses;

namespace ZWave
{
    public enum RequestNeighborUpdateStatus
    {
        Started = 0x21,
        Done = 0x22,
        Failed = 0x23,
    }

    public class RequestNeighborUpdateReport : NodeReport
    {
        public readonly RequestNeighborUpdateStatus Status;

        public RequestNeighborUpdateReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 2)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            // why is this node sometimes different from the source node (Node.NodeID)???    
            var nodeID = payload[0];
            Status = (RequestNeighborUpdateStatus)payload[1];
        }

        public bool IsCompleted
        {
            get { return Status == RequestNeighborUpdateStatus.Done || Status == RequestNeighborUpdateStatus.Failed; }
        }

        public override string ToString()
        {
            return $"Status:{Status}";
        }
    }
}
