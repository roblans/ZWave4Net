using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class MultiChannel : CommandClassBase
    {
        // This dictionary maps each endpoint id and command class type to it's instance.
        // We cache this so we can notify the command classes on event.
        //
        private IDictionary<byte, IDictionary<Type, EndpointSupportedCommandClassBase>> _endpointCommandClasses = new Dictionary<byte, IDictionary<Type, EndpointSupportedCommandClassBase>>();

        public enum command
        {
            EndPointGet = 0x07,
            EndPointReport = 0x08,
            CapabilityGet = 0x09,
            CapabilityReport = 0x0a,
            Encap = 0x0d,
        }

        [Obsolete("Changed event is deprecated, please use the endpoint command classes instead.")]
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
            await Channel.Send(Node, new Command(Class, command.Encap, controllerID, endPointId, Convert.ToByte(CommandClass.SwitchBinary), Convert.ToByte(SwitchBinary.command.Set), value ? (byte)0xFF : (byte)0x00), CancellationToken.None);
        }

        [Obsolete("Get is deprecated, please use GetEndPointCommandClass instead.")]
        public async Task<MultiChannelReport> Get(byte endPointId)
        {
            if (endPointId == 0)
                throw new ArgumentException("Endpoint id must be grater then 0.", nameof(endPointId));

            var controllerID = await Node.Controller.GetNodeID();
            var response = await Channel.Send(Node, new Command(Class, command.Encap, controllerID, endPointId, Convert.ToByte(CommandClass.SwitchBinary), Convert.ToByte(SwitchBinary.command.Get)), command.Encap, CancellationToken.None);
            return new MultiChannelReport(Node, response);
        }

        public Task<MultiChannelEndPointReport> DiscoverEndpoints()
        {
            return DiscoverEndpoints(CancellationToken.None);
        }

        public async Task<MultiChannelEndPointReport> DiscoverEndpoints(CancellationToken cancellationToken)
        {
            var response = await Channel.Send(Node, new Command(Class, command.EndPointGet), command.EndPointReport, cancellationToken);
            return new MultiChannelEndPointReport(Node, response);
        }

        public Task<MultiChannelCapabilityReport> GetEndPointCapabilities(byte endPointId)
        {
            return GetEndPointCapabilities(endPointId, CancellationToken.None);
        }

        public async Task<MultiChannelCapabilityReport> GetEndPointCapabilities(byte endPointId, CancellationToken cancellationToken)
        {
            if (endPointId == 0)
                throw new ArgumentException("Endpoint id must be grater then 0.", nameof(endPointId));

            var response = await Channel.Send(Node, new Command(Class, command.CapabilityGet, endPointId), command.CapabilityReport, cancellationToken);
            return new MultiChannelCapabilityReport(Node, response);
        }

        public T GetEndPointCommandClass<T>(byte endPointId) where T : EndpointSupportedCommandClassBase
        {
            if (endPointId == 0)
                throw new ArgumentException("Endpoint id must be grater then 0.", nameof(endPointId));

            lock(_endpointCommandClasses)
            {
                // Search for cached instance. If exist, return it.
                //
                if (_endpointCommandClasses.ContainsKey(endPointId) && _endpointCommandClasses[endPointId].ContainsKey(typeof(T)))
                {
                    return (T)_endpointCommandClasses[endPointId][typeof(T)];
                }

                // All supported command classes must have constructor that accept 2 arguments - the node and the endpoint id.
                //
                ConstructorInfo constructor = GetCommandClassConstructor<T>();
                if (constructor == null)
                {
                    throw new NotSupportedException($"{GetType()} not supporting command class {typeof(T)}");
                }

                T res = (T)constructor.Invoke(new object[] { Node, endPointId });

                // Cache this instance for future use.
                //
                if (!_endpointCommandClasses.ContainsKey(endPointId))
                {
                    _endpointCommandClasses[endPointId] = new Dictionary<Type, EndpointSupportedCommandClassBase>();
                }

                _endpointCommandClasses[endPointId][typeof(T)] = res;
                return res;
            }
        }

        private static ConstructorInfo GetCommandClassConstructor<T>() where T : EndpointSupportedCommandClassBase
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

            if (command.Payload.Length < 4)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(command.Payload)}");

            byte endPointId = command.Payload[0];
            CommandClass commandClass = (CommandClass)command.Payload[2];
            lock(_endpointCommandClasses)
            {
                if (_endpointCommandClasses.ContainsKey(endPointId))
                {
                    EndpointSupportedCommandClassBase endpointCommandClass = _endpointCommandClasses[endPointId].Values.FirstOrDefault(cc => cc.Class == commandClass);
                    if (endpointCommandClass != null)
                    {
                        endpointCommandClass.HandleEndpointReport(command.Payload.Skip(4).ToArray(), command.Payload[3]);
                    }
                }
            }

            // This part is for backward compatibility.
            //
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
