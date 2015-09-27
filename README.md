# ZWave4Net
 ZWave4Net is a .NET library that interfaces with the Aeotec / Aeon Labs Z-Stick. It uses an event-driven, non-blocking model that makes it lightweight and efficient.

Supported Targets:

- Managed Framework: net45
- Universal Windows: uap10.0
- Portable Class Library: net45 + win8

Supported Z-Wave command classes:

- Basic
- SwitchBinary
- SensorBinary
- SensorMultiLevel
- Meter
- Configuration
- Alarm
- ManufacturerSpecific
- Battery
- WakeUp
- Association
- Version
- SensorAlarm
 
FIBARO Wall Plug sample:

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


