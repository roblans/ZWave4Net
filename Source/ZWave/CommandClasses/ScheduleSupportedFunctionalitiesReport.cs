using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZWave.Channel;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class ScheduleSupportedFunctionalitiesReport : NodeReport
    {
        public readonly byte NumberOfSupportedIds;
        public readonly bool StartTimeSupportsNow;
        public readonly bool StartTimeSupportsHourAndMinute;
        public readonly bool StartTimeSupportsCalendarTime;
        public readonly bool StartTimeSupportsWeekdays;
        public readonly bool StartTimeSupportsTimeFromNow; // Schedule v3
        public readonly bool StartTimeSupportsRecurringMode; // Schedule v3
        public readonly bool SupportsEnableDisableSchedules;
        public readonly bool SupportsFallbackSchedule;
        public readonly bool SupportsOverrideSchedule;
        public readonly bool SupportsOverrideTypeAdvance;
        public readonly bool SupportsOverrideTypeRunForever;
        public readonly ScheduleSupportedCommandClass[] SupportedCommandClasses;
        public readonly byte ScheduleIdBlock = 1; // Schedule v2 (v1 has default value of 1)
        public readonly byte NumberOfScheduleIdBlocks = 1; // Schedule v2 (v1 has default value of 1)

        internal ScheduleSupportedFunctionalitiesReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));

            if (payload.Length < 4)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            byte numberOfSupportedCommandClasses = payload[2];

            if (payload.Length < 4 + numberOfSupportedCommandClasses * 2)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            NumberOfSupportedIds = payload[0];
            StartTimeSupportsNow = (payload[1] & 0b00000001) != 0;
            StartTimeSupportsHourAndMinute = (payload[1] & 0b00000010) != 0;
            StartTimeSupportsCalendarTime = (payload[1] & 0b00000100) != 0;
            StartTimeSupportsWeekdays = (payload[1] & 0b00001000) != 0;
            StartTimeSupportsTimeFromNow = (payload[1] & 0b00010000) != 0;
            StartTimeSupportsRecurringMode = (payload[1] & 0b00100000) != 0;
            SupportsFallbackSchedule = (payload[1] & 0b01000000) != 0;
            SupportsEnableDisableSchedules = (payload[1] & 0b10000000) != 0;

            SupportedCommandClasses = new ScheduleSupportedCommandClass[numberOfSupportedCommandClasses];
            for (byte commandClassIndex = 0; commandClassIndex < numberOfSupportedCommandClasses; commandClassIndex++)
            {
                SupportedCommandClasses[commandClassIndex] =
                    new ScheduleSupportedCommandClass(payload[3 + commandClassIndex * 2], payload[4 + commandClassIndex * 2]);
            }

            byte overrideSettingByte = payload[3 + numberOfSupportedCommandClasses * 2];
            SupportsOverrideTypeAdvance = (overrideSettingByte & 0b00000001) != 0;
            SupportsOverrideTypeRunForever = (overrideSettingByte & 0b00000010) != 0;
            SupportsOverrideSchedule = (overrideSettingByte & 0b10000000) != 0;

            // Schedule v2 - Schedule ID Block
            if (payload.Length == 6 + numberOfSupportedCommandClasses * 2)
            {
                ScheduleIdBlock = payload[4 + numberOfSupportedCommandClasses * 2];
                NumberOfScheduleIdBlocks = payload[5 + numberOfSupportedCommandClasses * 2];
            }
        }

        public override string ToString()
        {
            return $"# IDs:{NumberOfSupportedIds}";
        }
    }
}
