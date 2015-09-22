using System;
using System.Collections.Generic;
using System.Text;
using ZWave.Driver.Communication;

namespace ZWave.Driver.CommandClasses
{
    public interface ICommandClass
    {
        Node Node { get; }
        CommandClass Class { get; }
    }
}
