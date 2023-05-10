![](https://img.shields.io/vso/build/roblans/df0c356b-e9f5-4364-bdf2-3dde5ed0dc05/7.svg) [![](https://img.shields.io/nuget/v/zwave4net.svg)](https://www.nuget.org/packages/ZWave4Net/)

# ZWave4Net
 ZWave4Net is a .NET library that interfaces with the Aeotec / Aeon Labs Z-Stick. It uses an event-driven, non-blocking model that makes it lightweight and efficient.

Supported Targets:

- .NET 6.0
- .NET 5.0
- .NET 4.8
- .NET Standard 2.0
- .NET Standard 2.1
- .NET Core 3.1
- Universal App Platform: win10

Runs on Raspberry PI IoT Windows 10 (see note below)

NuGet package: https://www.nuget.org/packages/ZWave4Net/

Supported Z-Wave command classes:

- Alarm v1-2
- Association v1-3
- Basic v1-2
- Battery v1*
- CentralScene v1*
- Clock v1
- Color v1-3
- Configuration v1*
- ManufacturerSpecific v1-2
- Meter v1-6
- MultiChannel
- MultiChannelAssociation
- NodeNaming v1
- Notification v3-8
- SceneActivation v1
- Schedule v1
- SensorAlarm v1
- SensorBinary v1-2
- SensorMultiLevel v1-11
- SwitchAll v1
- SwitchBinary v1-2
- SwitchMultiLevel v1-2, 4*
- SwitchToggleBinary v1
- SwitchToggleMultiLevel v1
- ThermostatFanMode v1*
- ThermostatMode v1*
- ThermostatOperatingState v1*
- ThermostatSetpoint v1*
- Version v1-2*
- WakeUp v1-3
 
FIBARO Wall Plug sample:

```cs
        public async Task TurnWallPlugOn()
        {
            // the nodeID of the wallplug
            byte wallPlugNodeID = 3;

            // create the controller
            var controller = new ZWaveController("COM1");
            
            // open the controller
            controller.Open();

            // get the included nodes
            var nodes = await controller.GetNodes();
            
            // get the wallplug
            var wallPlug = nodes[wallPlugNodeID];
            
            // get the SwitchBinary commandclass
            var switchBinary = wallPlug.GetCommandClass<SwitchBinary>();

            // turn wallplug on
            await switchBinary.Set(0xFF);

            // close the controller
            controller.Close();
        }
```

FIBARO Motion Sensor sample:

```cs
        public async Task SensorAlarm()
        {
            // the nodeID of the motion sensor
            byte motionSensorID = 5;

            // create the controller
            var controller = new ZWaveController("COM1");

            // open the controller
            controller.Open();

            // get the included nodes
            var nodes = await controller.GetNodes();

            // get the motionSensor
            var motionSensor = nodes[motionSensorID];

            // get the SensorAlarm commandclass
            var sensorAlarm = motionSensor.GetCommandClass<SensorAlarm>();

            // subscribe to alarm event
            sensorAlarm.Changed += (s, e) => Console.WriteLine("Alarm");

            // wait
            Console.ReadLine();

            // close the controller
            controller.Close();
        }
```

Note: running ZWave4Net on Raspberry PI IoT Windows 10:

```cs
    // note: opening the serialport by name fails on Windows 10 IoT, use USB vendorId and productId instead
    var controller = new ZWaveController(vendorId: 0x0658, productId: 0x0200);
```
