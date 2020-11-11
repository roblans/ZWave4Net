using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZWave.Channel;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class ScheduleStateReport : NodeReport
    {
        public readonly byte NumberOfSupportedIds;
        public readonly bool OverrideActive;
        public readonly ScheduleState[] ScheduleStates;

        internal ScheduleStateReport(Node node, IEnumerable<byte[]> payloads) : base(node)
        {
            if (payloads == null)
                throw new ArgumentNullException(nameof(payloads));
            if (payloads.Count() == 0)
                throw new ArgumentOutOfRangeException(nameof(payloads));

            var firstPayload = payloads.First();
            NumberOfSupportedIds = firstPayload[0];
            OverrideActive = (firstPayload[1] & 0b00000001) != 0;

            var stateList = new List<ScheduleState>();
            foreach (var payload in payloads)
            {
                Process(payload, stateList);
            }
            ScheduleStates = stateList.ToArray();
        }

        private void Process(byte[] payload, List<ScheduleState> stateList)
        {
            for (int i = 2; i < payload.Length; i++)
            {
                stateList.Add((ScheduleState)(payload[i] & 0b00001111));
                stateList.Add((ScheduleState)(payload[i] >> 4));
            }
        }

        public override string ToString()
        {
            var enabledStates = ScheduleStates
                .Select((v, i) => new { state = v, index = i })
                .Where(item => item.state != ScheduleState.NotUsed)
                .Select(item => $"#{item.index + 1}");

            return $"Override: {OverrideActive}, used Schedules: [{string.Join(",", enabledStates)}]";
        }
    }
}
