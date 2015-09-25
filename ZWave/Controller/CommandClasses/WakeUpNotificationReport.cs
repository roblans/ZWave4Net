using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using ZWave.Communication;

namespace ZWave.Controller.CommandClasses
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
