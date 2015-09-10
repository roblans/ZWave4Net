﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net.Commands
{
    public class WakeUp : CommandClass
    {
        enum wakeUpCmd
        {
            IntervalSet = 0x04,
            IntervalGet = 0x05,
            IntervalReport = 0x06,
            Notification = 0x07,
            NoMoreInformation = 0x08
        }

        public WakeUp(Node node) : base(0x84, node)
        {
        }

        protected override Enum[] Commands
        {
            get { return Enum.GetValues(typeof(wakeUpCmd)).Cast<Enum>().ToArray(); }
        }

        public async Task<byte> GetInterval()
        {
            var response = await Invoker.Send(new Command(ClassID, wakeUpCmd.IntervalGet));
            return response.Payload.First();
        }

        public Task SetInterval(byte value)
        {
            return Invoker.Send(new Command(ClassID, wakeUpCmd.IntervalSet, value));
        }

        protected override bool IsCorrelated(Enum request, Enum response)
        {
            if (object.Equals(request, wakeUpCmd.IntervalGet) && object.Equals(response, wakeUpCmd.IntervalReport))
                return true;

            return false;
        }

        protected override void OnEvent(Enum command, byte[] payload)
        {
            Platform.LogMessage(LogLevel.Debug, string.Format($"Event: Node = {Node}, Class = {ClassName}, Command = {command}, {BitConverter.ToString(payload)}"));
        }
    }
}
