using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net.Commands
{
    interface ICommandInvoker
    {
        Task<Command> Invoke(Command command);
    }
}
