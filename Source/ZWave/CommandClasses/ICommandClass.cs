using System;
using System.Collections.Generic;
using System.Text;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public interface ICommandClass
    {
        Node Node { get; }
        CommandClass Class { get; }
    }
}
