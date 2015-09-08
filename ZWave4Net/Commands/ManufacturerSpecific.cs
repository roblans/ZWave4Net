using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net.Commands
{
    public class ManufacturerSpecific : CommandClass
    {
        enum manufacturerSpecificCmd
        {
            Get = 0x04,
            Report = 0x05
        }

        public ManufacturerSpecific(Node node) : base(0x72, node)
        {
        }

        protected override Enum[] Commands
        {
            get { return Enum.GetValues(typeof(manufacturerSpecificCmd)).Cast<Enum>().ToArray(); }
        }

        public async Task<ProductData> GetProductData()
        {
            var response = await Invoker.Invoke(new Command(ClassID, manufacturerSpecificCmd.Get));
            return ProductData.Parse(response.Payload);
        }

        protected override bool IsCorrelated(Enum request, Enum response)
        {
            if (object.Equals(request, manufacturerSpecificCmd.Get) && object.Equals(response, manufacturerSpecificCmd.Report))
                return true;

            return false;
        }

        protected override void OnResponse(Enum response, byte[] payload)
        {
            var productInfo = ProductData.Parse(payload);
            Platform.Log(LogLevel.Info, string.Format($"Response: Node = {Node}, Class = {ClassName}, Command = {response}, {productInfo}"));
        }

        protected override void OnEvent(Enum @event, byte[] payload)
        {
            var productInfo = ProductData.Parse(payload);
            Platform.Log(LogLevel.Info, string.Format($"Event: Node = {Node}, Class = {ClassName}, Command = {@event}, {productInfo}"));
        }
    }
}
