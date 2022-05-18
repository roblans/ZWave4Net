using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZWave.CommandClasses;
using ZWave.Devices.Fibaro;

namespace ZWave.Devices.Zipato
{
    public class RgbwLightBulb : Device
    {
        public RgbwLightBulb(Node node)
            : base(node)
        {
        }

        public async Task SwitchOn()
        {
            await Node.GetCommandClass<Basic>().Set(0xFF);
        }

        public async Task SwitchOff()
        {
            await Node.GetCommandClass<Basic>().Set(0x00);
        }

        public async Task AddAssociation(AssociationGroup group, Node node)
        {
            await Node.GetCommandClass<Association>().Add((byte)group, node.NodeID);
        }

        public async Task RemoveAssociation(AssociationGroup group, Node node)
        {
            await Node.GetCommandClass<Association>().Remove((byte)group, node.NodeID);
        }

        public async Task SetColor(byte warmWhite, byte coldWhite, byte red, byte green, byte blue)
        {
            var components = new List<ColorComponent>();
            components.Add(new ColorComponent(Convert.ToByte(LightBulbColorComponent.WarmWhite), warmWhite));
            components.Add(new ColorComponent(Convert.ToByte(LightBulbColorComponent.ColdWhite), coldWhite));
            components.Add(new ColorComponent(Convert.ToByte(LightBulbColorComponent.Red), red));
            components.Add(new ColorComponent(Convert.ToByte(LightBulbColorComponent.Green), green));
            components.Add(new ColorComponent(Convert.ToByte(LightBulbColorComponent.Blue), blue));

            await Node.GetCommandClass<Color>().Set(components.ToArray());
        }

        public async Task<byte> GetColor(LightBulbColorComponent component)
        {
            var response = await Node.GetCommandClass<Color>().Get(Convert.ToByte(component));
            return response.Component.Value;
        }

        public async Task<byte> GetColorTemperature()
        {
            return (byte)(await Node.GetCommandClass<Configuration>().Get(1)).Value;
        }

        public async Task SetColorTemperature(byte value)
        {
            if (value < 1 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Value must be between 1 and 100.");

            await Node.GetCommandClass<Configuration>().Set(1, value);
        }

        public async Task<byte> GetShockSensorSensitivity()
        {
            return (byte)(await Node.GetCommandClass<Configuration>().Get(2)).Value;
        }

        public async Task SetShockSensorSensitivity(byte value)
        {
            if (value < 0 || value > 31)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Value must be between 0 and 31.");

            await Node.GetCommandClass<Configuration>().Set(2, value);
        }

        public enum LightBulbColorComponent : byte
        {
            WarmWhite= 0x00,
            ColdWhite = 0x01,
            Red = 0x02,
            Green = 0x03,
            Blue = 0x04,
        }
    }
}
