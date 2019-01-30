using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationValidator.Models
{
    public class Node
    {

        public byte NodeID { get; set; }
        public CommandClassCommunication[] CommandClassCommunications { get; set; }

    }

    public class CommandClassCommunication
    {
        public string CommandClass { get; set; }

        public int RequestsSucceeded { get; set; }
        public int RequestsFailed { get; set; }

        public int EventReceived { get; set; }
    }
}
