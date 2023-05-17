using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.CommandClasses;

namespace ZWave.Devices.Qubino
{
    public class FlushShutter : Device
    {
        private readonly SwitchBinary _switchUp;
        private readonly SwitchBinary _switchDown;
        private readonly SwitchMultiLevel _switchMultiLevel;
        private readonly SensorMultiLevel _sensorMultiLevel;

        public event EventHandler<PositionEventArgs> PositionChanged;

        public FlushShutter(Node node)
            : base(node)
        {
            var multiChannel = node.GetCommandClass<MultiChannel>();
            _switchUp = multiChannel.GetEndPointCommandClass<SwitchBinary>(1);
            _switchDown = multiChannel.GetEndPointCommandClass<SwitchBinary>(2);
            _switchMultiLevel = node.GetCommandClass<SwitchMultiLevel>();
            _switchMultiLevel.Changed += switchMultiLevel_Changed;
            _sensorMultiLevel = node.GetCommandClass<SensorMultiLevel>();
        }

        private void switchMultiLevel_Changed(object sender, ReportEventArgs<SwitchMultiLevelReport> e)
        {
            OnPositionChanged(new PositionEventArgs(e.Report.TargetValue));
        }

        protected virtual void OnPositionChanged(PositionEventArgs e)
        {
            PositionChanged?.Invoke(this, e);
        }

        public async Task MoveUp(TimeSpan duration)
        {
            await _switchUp.Set(true);
            await Task.Delay(duration);
            await _switchUp.Set(false);
        }

        public async Task MoveDown(TimeSpan duration)
        {
            await _switchDown.Set(true);
            await Task.Delay(duration);
            await _switchDown.Set(false);
        }

        public async Task SetPosition(byte position)
        {
            await _switchMultiLevel.Set(position);
        }

        public async Task<SwitchMultiLevelReport> GetPosition()
        {
            return await _switchMultiLevel.Get();
        }

        public async Task<SensorMultiLevelReport> GetTemperature()
        {
            return await _sensorMultiLevel.Get(SensorType.Temperature, 0);
        }
    }

    public class PositionEventArgs : EventArgs
    {
        public readonly byte Value;

        public PositionEventArgs(byte value)
        {
            Value = value;
        }
    }
}
