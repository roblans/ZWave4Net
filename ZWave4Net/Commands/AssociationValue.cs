using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net.Commands
{
    public class AssociationValue
    {
        public byte GroupID { get; private set; }
        public byte MaxAssociations { get; private set; }
        public byte NumReportsToFollow { get; private set; }
        public byte[] AssociatedNodes { get; private set; }

        public static AssociationValue Parse(byte[] payload)
        {
            var value = new AssociationValue();
            value.GroupID = payload[0];
            value.MaxAssociations = payload[1];
            value.NumReportsToFollow = payload[2];
            value.AssociatedNodes = payload.Skip(3).ToArray();
            return value;
        }
    }
}
