using System;
using System.Collections.Generic;
using System.Text;
using ZWave.Communication;

namespace ZWave.Controller.CommandClasses
{
    public interface ICommandClass
    {
        Node Node { get; }
        CommandClass Class { get; }
    }
}
