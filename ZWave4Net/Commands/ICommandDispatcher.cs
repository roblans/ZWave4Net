using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave4Net.Communication;

namespace ZWave4Net.Commands
{
    interface ICommandDispatcher
    {
        Task<Command> Send(Command command, Enum response);
        Task Send(Command command);
    }
}
