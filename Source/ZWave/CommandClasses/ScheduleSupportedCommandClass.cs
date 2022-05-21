using System;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class ScheduleSupportedCommandClass
    {
        public readonly CommandClass Class;
        public readonly bool SupportsGet;
        public readonly bool SupportsSet;
        public readonly bool SupportsCustomCommands;

        private const byte SupportsBothValue = 0x0;
        private const byte SupportsSetOnlyValue = 0x1;
        private const byte SupportsGetOnlyValue = 0x2;
        private const byte SupportsCustomCommandsValue = 0x3; // Schedule v4

        internal ScheduleSupportedCommandClass(byte commandClass, byte supported)
        {

            Class = (CommandClass)Enum.ToObject(typeof(CommandClass), commandClass);
            SupportsGet = supported == SupportsBothValue || supported == SupportsGetOnlyValue;
            SupportsSet = supported == SupportsBothValue || supported == SupportsSetOnlyValue;
            SupportsCustomCommands = supported == SupportsCustomCommandsValue;
        }

        public override string ToString()
        {
            return $"{Class}, Set:{SupportsSet} Get:{SupportsGet}";
        }
    }
}
