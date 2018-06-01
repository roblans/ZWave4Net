using System;
using System.Reflection;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class MultiChannel : CommandClassBase
    {
        public enum command
        {
            EndPointGet = 0x07,
            EndPointReport = 0x08,
            CapabilityGet = 0x09,
            CapabilityReport = 0x0a,
            Encap = 0x0d,
        }

        public event EventHandler<ReportEventArgs<MultiChannelReport>> Changed;

        public MultiChannel(Node node)
            : base(node, CommandClass.MultiChannel)
        {
        }

        [Obsolete("BinarySwitchSet is deprecated, please use GetEndPointCommandClass instead.")]
        public async Task BinarySwitchSet(byte endPointId, bool value)
        {
            if (endPointId == 0)
                throw new ArgumentException("Endpoint id must be grater then 0.", nameof(endPointId));

            var controllerID = await Node.Controller.GetNodeID();
            await Channel.Send(Node, new Command(Class, command.Encap, controllerID, endPointId, Convert.ToByte(CommandClass.SwitchBinary), Convert.ToByte(SwitchBinary.command.Set), value ? (byte)0xFF : (byte)0x00));
        }

        [Obsolete("Get is deprecated, please use GetEndPointCommandClass instead.")]
        public async Task<MultiChannelReport> Get(byte endPointId)
        {
            if (endPointId == 0)
                throw new ArgumentException("Endpoint id must be grater then 0.", nameof(endPointId));

            var controllerID = await Node.Controller.GetNodeID();
            var response = await Channel.Send(Node, new Command(Class, command.Encap, controllerID, endPointId, Convert.ToByte(CommandClass.SwitchBinary), Convert.ToByte(SwitchBinary.command.Get)), command.Encap);
            return new MultiChannelReport(Node, response);
        }

        public async Task<MultiChannelEndPointReport> DiscoverEndpoints()
        {
            var response = await Channel.Send(Node, new Command(Class, command.EndPointGet), command.EndPointReport);
            return new MultiChannelEndPointReport(Node, response);
        }

        public async Task<MultiChannelCapabilityReport> GetEndPointCapabilities(byte endPointId)
        {
            if (endPointId == 0)
                throw new ArgumentException("Endpoint id must be grater then 0.", nameof(endPointId));

            var response = await Channel.Send(Node, new Command(Class, command.CapabilityGet, endPointId), command.CapabilityReport);
            return new MultiChannelCapabilityReport(Node, response);
        }

        public T GetEndPointCommandClass<T>(byte endPointId) where T : ICommandClass
        {
            if (endPointId == 0)
                throw new ArgumentException("Endpoint id must be grater then 0.", nameof(endPointId));

            // All supported command classes must have constructor that accept 2 arguments - the node and the endpoint id.
            //
            ConstructorInfo constructor = GetCommandClassConstructor<T>();
            if (constructor == null)
            {
                throw new NotSupportedException($"{GetType()} not supporting command class {typeof(T)}");
            }

            return (T)constructor.Invoke(new object[] { Node, endPointId });
        }

        private static ConstructorInfo GetCommandClassConstructor<T>() where T : ICommandClass
        {
            foreach (ConstructorInfo ctr in typeof(T).GetTypeInfo().DeclaredConstructors)
            {
                ParameterInfo[] constructorParameters = ctr.GetParameters();
                if (constructorParameters.Length == 2 && constructorParameters[0].ParameterType == typeof(Node) && constructorParameters[1].ParameterType == typeof(byte))
                {
                    return ctr;
                }
            };

            return null;
        }

        protected internal override void HandleEvent(Command command)
        {
            base.HandleEvent(command);

            var report = new MultiChannelReport(Node, command.Payload);
            OnChanged(new ReportEventArgs<MultiChannelReport>(report));
        }

        protected virtual void OnChanged(ReportEventArgs<MultiChannelReport> e)
        {
            var handler = Changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
