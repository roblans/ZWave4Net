using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using ZWave.Driver.Communication;

namespace ZWave.Driver.CommandClasses
{
    public class WakeUpNotificationReport : NodeReport
    {
        internal WakeUpNotificationReport(Node node) : base(node)
        {
        }

        public override string ToString()
        {
            return $"Notification";
        }
    }
}
