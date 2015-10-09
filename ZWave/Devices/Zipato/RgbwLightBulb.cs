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

        public event EventHandler<EventArgs> SwitchedOn;
        public event EventHandler<EventArgs> SwitchedOff;

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
            components.Add(new ColorComponent(0, warmWhite));
            components.Add(new ColorComponent(1, coldWhite));
            components.Add(new ColorComponent(2, red));
            components.Add(new ColorComponent(3, green));
            components.Add(new ColorComponent(4, blue));

            await Node.GetCommandClass<Color>().Set(components.ToArray());
        }

        protected virtual void OnSwitchedOn(EventArgs e)
        {
            SwitchedOn?.Invoke(this, e);
        }

        protected virtual void OnSwitchedOff(EventArgs e)
        {
            SwitchedOff?.Invoke(this, e);
        }
    }
}
