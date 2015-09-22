using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave.Driver.CommandClasses
{
    public class ReportReceivedEventArgs<T> where T : NodeReport
    {
        public readonly T Report;

        public ReportReceivedEventArgs(T report)
        {
            Report = report;
        }
    }
}
