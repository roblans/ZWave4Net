using System;
using ZWave.Channel;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class VersionCommandClassReport : NodeReport
    {
        public readonly CommandClass Class;
        public readonly byte Version;

        internal VersionCommandClassReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 2)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            Class = (CommandClass)Enum.ToObject(typeof(CommandClass), payload[0]);
            Version = payload[1];

            if (Class == CommandClass.Notification && Version < 3)
                Class = CommandClass.Alarm;
            else if (Class == CommandClass.Alarm && Version > 2)
                Class = CommandClass.Notification;
        }

        internal static Func<byte[], bool> GetResponseValidatorForCommandClass(Node node, CommandClass @class)
        {
            return payload =>
            {
                try
                {
                    var report = new VersionCommandClassReport(node, payload);
                    return report.Class == @class;
                }
                catch
                {
                    return false;
                }
            };
        }

        public override string ToString()
        {
            return $"Class:{Class}, Version:{Version}";
        }
    }
}
