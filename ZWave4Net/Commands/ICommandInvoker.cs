using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave4Net.Communication;

namespace ZWave4Net.Commands
{
    interface ICommandInvoker
    {
        Task<Command> Send(Command command, Enum replyCommand);
    }
}
