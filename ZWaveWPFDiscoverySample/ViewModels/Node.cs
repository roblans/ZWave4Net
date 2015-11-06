using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZWave.CommandClasses;

namespace ZWaveWPFDiscoverySample.ViewModels
{
    public class Node : ViewModel
    {

        private readonly ZWave.Node node;

        public bool FoundNode { get; private set; }
        public bool WaitingForWakeUp { get; private set; }
        public bool LoadingInformation { get; private set; }

        public byte NodeID { get; private set; }
        public string NodeType { get; private set; }

        private ushort ManufacturerID;
        private ushort ProductID;

        public string Device { get { return $"Manu: {ManufacturerID}, Prd: {ProductID}"; } }

        public ObservableCollection<NodeValue> Values { get; private set; }

        public Node(ZWave.Node node)
        {
            this.Values = new ObservableCollection<NodeValue>();

            this.node = node;
            NodeID = node.NodeID;

            FoundNode = false;
            WaitingForWakeUp = true;

            Subscribe();
            GetPrimaryInfo();
            GetNodeDetails();
        }



        public async void GetPrimaryInfo()
        {
            try
            {
                LoadingInformation = true;
                Notify(nameof(LoadingInformation));

                var protocolInfo = await node.GetProtocolInfo();
                NodeType = protocolInfo.GenericType.ToString();
                Notify(nameof(NodeType));
            }
            catch
            { }
            finally
            {
                LoadingInformation = false;
                Notify(nameof(LoadingInformation));
            }
        }
        public async void GetNodeDetails()
        {
            try
            {
                LoadingInformation = true;
                Notify(nameof(LoadingInformation));

                WaitingForWakeUp = false;
                Notify(nameof(WaitingForWakeUp));

                var protocolInfo = await node.GetProtocolInfo();
                NodeType = protocolInfo.GenericType.ToString();
                Notify(nameof(NodeType));

                var manuInfo = await node.GetCommandClass<ManufacturerSpecific>().Get();
                ManufacturerID = manuInfo.ManufacturerID;
                ProductID = manuInfo.ProductID;
                
                Notify(nameof(Device));

                FoundNode = true;
                Notify(nameof(FoundNode));
            }
            catch
            {
                WaitingForWakeUp = true;
                Notify(nameof(WaitingForWakeUp));
            }
            finally
            {
                LoadingInformation = false;
                Notify(nameof(LoadingInformation));
            }
        }




        private void Subscribe()
        {
            var basic = node.GetCommandClass<Basic>();
            basic.Changed += (_, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var type = "basic";
                    var value = e.Report.Value;
                    if (Values.Any(el => el.Type == type))
                        Values.First(val => val.Type == type).SetValue(value);
                    else
                        Values.Add(new NodeValue(type, value));
                });

                if (WaitingForWakeUp)
                    GetNodeDetails();
            };

            var sensorMultiLevel = node.GetCommandClass<SensorMultiLevel>();
            sensorMultiLevel.Changed += (_, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var type = e.Report.Type.ToString();
                    var value = $"{e.Report.Value} {e.Report.Unit}";
                    if (Values.Any(el => el.Type == type))
                        Values.First(val => val.Type == type).SetValue(value);
                    else
                        Values.Add(new NodeValue(type, value));
                });

                if (WaitingForWakeUp)
                    GetNodeDetails();
            };

            var meter = node.GetCommandClass<Meter>();
            meter.Changed += (_, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var type = e.Report.Type.ToString();
                    var value = $"{e.Report.Value} {e.Report.Unit}";
                    if (Values.Any(el => el.Type == type))
                        Values.First(val => val.Type == type).SetValue(value);
                    else
                        Values.Add(new NodeValue(type, value));
                });

                if (WaitingForWakeUp)
                    GetNodeDetails();
            };

            var alarm = node.GetCommandClass<Alarm>();
            alarm.Changed += (_, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var type = e.Report.Type.ToString();
                    var value = e.Report.Level;
                    if (Values.Any(el => el.Type == type))
                        Values.First(val => val.Type == type).SetValue(value);
                    else
                        Values.Add(new NodeValue(type, value));
                });

                if (WaitingForWakeUp)
                    GetNodeDetails();
            };

            var sensorBinary = node.GetCommandClass<SensorBinary>();
            sensorBinary.Changed += (_, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var type = "sensor";
                    var value = e.Report.Value;
                    if (Values.Any(el => el.Type == type))
                        Values.First(val => val.Type == type).SetValue(value);
                    else
                        Values.Add(new NodeValue(type, value));
                });

                if (WaitingForWakeUp)
                    GetNodeDetails();
            };

            var sensorAlarm = node.GetCommandClass<SensorAlarm>();
            sensorAlarm.Changed += (_, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var type = e.Report.Type.ToString();
                    var value = e.Report.Level;
                    if (Values.Any(el => el.Type == type))
                        Values.First(val => val.Type == type).SetValue(value);
                    else
                        Values.Add(new NodeValue(type, value));
                });

                if (WaitingForWakeUp)
                    GetNodeDetails();
            };

            var wakeUp = node.GetCommandClass<WakeUp>();
            wakeUp.Changed += (_, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var type = "wake-up";
                    var value = e.Report.Awake;
                    if (Values.Any(el => el.Type == type))
                        Values.First(val => val.Type == type).SetValue(value);
                    else
                        Values.Add(new NodeValue(type, value));
                });

                if (WaitingForWakeUp)
                    GetNodeDetails();
            };

            var switchBinary = node.GetCommandClass<SwitchBinary>();
            switchBinary.Changed += (_, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var type = "switch";
                    var value = e.Report.Value;
                    if (Values.Any(el => el.Type == type))
                        Values.First(val => val.Type == type).SetValue(value);
                    else
                        Values.Add(new NodeValue(type, value));
                });

                if (WaitingForWakeUp)
                    GetNodeDetails();
            };

            var multiChannel = node.GetCommandClass<MultiChannel>();
            multiChannel.Changed += (_, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var type = $"multi channel {e.Report.EndPointID}";
                    var value = e.Report.Report.ToString();
                    if (Values.Any(el => el.Type == type))
                        Values.First(val => val.Type == type).SetValue(value);
                    else
                        Values.Add(new NodeValue(type, value));
                });

                if (WaitingForWakeUp)
                    GetNodeDetails();
            };

            var thermostatSetpoint = node.GetCommandClass<ThermostatSetpoint>();
            thermostatSetpoint.Changed += (_, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var type = "temp setpoint";
                    var value = $"{e.Report.Value} {e.Report.Unit}";
                    if (Values.Any(el => el.Type == type))
                        Values.First(val => val.Type == type).SetValue(value);
                    else
                        Values.Add(new NodeValue(type, value));
                });

                if (WaitingForWakeUp)
                    GetNodeDetails();
            };
        }
    }
}
