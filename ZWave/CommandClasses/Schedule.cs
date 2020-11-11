using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class Schedule : EndpointSupportedCommandClassBase
    {
        private const int ScheduleIdBlockMinimalProtocolVersion = 2;

        public const byte AllSchedules = 0x00;
        public const byte ScheduleIdFallback = 0xFE;
        public const byte ScheduleIdOverride = 0xFF;

        enum command
        {
            SupportedGet = 0x01,
            SupportedReport = 0x02,
            Set = 0x03,
            Get = 0x04,
            Report = 0x05,
            Remove = 0x06,
            StateSet = 0x07,
            StateGet = 0x08,
            StateReport = 0x09,
            SupportedCommandsGet = 0x0A,
            SupportedCommandsReport = 0x0B
        }

        public Schedule(Node node)
            : base(node, CommandClass.Schedule)
        { }

        internal Schedule(Node node, byte endpointId)
            : base(node, CommandClass.Schedule, endpointId)
        { }

        public Task<ScheduleSupportedFunctionalitiesReport> GetSupportedFunctionalities()
        {
            return GetSupportedFunctionalities(CancellationToken.None);
        }

        public async Task<ScheduleSupportedFunctionalitiesReport> GetSupportedFunctionalities(CancellationToken cancellationToken)
        {
            var response = await Send(new Command(Class, command.SupportedGet), command.SupportedReport, cancellationToken);
            return new ScheduleSupportedFunctionalitiesReport(Node, response);
        }

        public Task<ScheduleSupportedFunctionalitiesReport> GetSupportedFunctionalities(byte scheduleIdBlock)
        {
            return GetSupportedFunctionalities(scheduleIdBlock, CancellationToken.None);
        }

        public async Task<ScheduleSupportedFunctionalitiesReport> GetSupportedFunctionalities(byte scheduleIdBlock, CancellationToken cancellationToken)
        {
            if (!await IsSupportScheduleIdBlock(cancellationToken))
            {
                throw new VersionNotSupportedException($"Schedule ID blocks work with class type {Class} greater or equal to {ScheduleIdBlockMinimalProtocolVersion}.");
            }

            var response = await Send(new Command(Class, command.SupportedGet, scheduleIdBlock), command.SupportedReport, cancellationToken);
            return new ScheduleSupportedFunctionalitiesReport(Node, response);
        }

        public Task<bool> IsSupportScheduleIdBlock()
        {
            return IsSupportScheduleIdBlock(CancellationToken.None);
        }

        public async Task<bool> IsSupportScheduleIdBlock(CancellationToken cancellationToken)
        {
            var report = await Node.GetCommandClassVersionReport(Class, cancellationToken);
            return report.Version >= ScheduleIdBlockMinimalProtocolVersion;
        }

        public Task<ScheduleReport> Get(byte scheduleId)
        {
            return Get(scheduleId, CancellationToken.None);
        }

        public async Task<ScheduleReport> Get(byte scheduleId, CancellationToken cancellationToken)
        {
            var response = await Send(new Command(Class, command.Get, scheduleId), command.Report, cancellationToken);
            return new ScheduleReport(Node, response);
        }

        public Task Set(byte scheduleId, ScheduleData data)
        {
            return Set(scheduleId, data, CancellationToken.None);
        }

        public async Task Set(byte scheduleId, ScheduleData data, CancellationToken cancellationToken)
        {
            await Send(new Command(Class, command.Set, data.ToPayload(scheduleId)), cancellationToken);
        }

        public Task Set(byte scheduleId, byte scheduleIdBlock, ScheduleData data)
        {
            return Set(scheduleId, scheduleIdBlock, data, CancellationToken.None);
        }

        public async Task Set(byte scheduleId, byte scheduleIdBlock, ScheduleData data, CancellationToken cancellationToken)
        {
            if (!await IsSupportScheduleIdBlock(cancellationToken))
            {
                throw new VersionNotSupportedException($"Schedule ID blocks work with class type {Class} greater or equal to {ScheduleIdBlockMinimalProtocolVersion}.");
            }

            await Send(new Command(Class, command.Set, data.ToPayload(scheduleId, scheduleIdBlock)), cancellationToken);
        }

        public Task Remove(byte scheduleId)
        {
            return Remove(scheduleId, CancellationToken.None);
        }

        public async Task Remove(byte scheduleId, CancellationToken cancellationToken)
        {
            await Send(new Command(Class, command.Remove), cancellationToken);
        }

        public Task Remove(byte scheduleId, byte scheduleIdBlock)
        {
            return Remove(scheduleId, scheduleIdBlock, CancellationToken.None);
        }

        public async Task Remove(byte scheduleId, byte scheduleIdBlock, CancellationToken cancellationToken)
        {
            if (!await IsSupportScheduleIdBlock(cancellationToken))
            {
                throw new VersionNotSupportedException($"Schedule ID blocks work with class type {Class} greater or equal to {ScheduleIdBlockMinimalProtocolVersion}.");
            }

            await Send(new Command(Class, command.Remove, scheduleIdBlock), cancellationToken);
        }

        public Task<ScheduleStateReport> GetState()
        {
            return GetState(CancellationToken.None);
        }

        public Task<ScheduleStateReport> GetState(CancellationToken cancellationToken)
        {
            return DoGetState(new Command(Class, command.StateGet), cancellationToken);
        }

        public Task<ScheduleStateReport> GetState(byte scheduleIdBlock)
        {
            return GetState(scheduleIdBlock, CancellationToken.None);
        }

        public Task<ScheduleStateReport> GetState(byte scheduleIdBlock, CancellationToken cancellationToken)
        {
            return DoGetState(new Command(Class, command.StateGet, scheduleIdBlock), cancellationToken);
        }

        public async Task<ScheduleStateReport> DoGetState(Command getStateCommand, CancellationToken cancellationToken)
        {
            var completion = new TaskCompletionSource<ScheduleStateReport>();
            var responses = new List<byte[]>();
            var node = Node;

            void onEventReceived(object sender, NodeEventArgs args)
            {
                if (args.Command.ClassID == (byte)CommandClass.Schedule && args.Command.CommandID == (byte)command.StateReport)
                {
                    responses.Add(args.Command.Payload);
                    int numberOfFollowupReports = args.Command.Payload[1] >> 1;
                    if (numberOfFollowupReports == 0)
                    {
                        var report = new ScheduleStateReport(node, responses);
                        completion.SetResult(report);
                    }
                }
            }

            Channel.NodeEventReceived += onEventReceived;
            try
            {
                await Send(getStateCommand, cancellationToken).ConfigureAwait(false);

                using (cancellationToken.Register(() =>
                {
                    completion.TrySetCanceled();
                }))
                {
                    return await completion.Task.ConfigureAwait(false);
                }
            }
            finally
            {
                Channel.NodeEventReceived -= onEventReceived;
            }
        }
    }
}
