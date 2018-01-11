#if NET
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Linq;
using System.Threading.Tasks;
using ZWave.CommandClasses;
using System.Globalization;

namespace ZWave.Net
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class ControllerRestService : IControllerRestService
    {
        public readonly ZWaveController Controller;

        public ControllerRestService(ZWaveController controller)
        {
            Controller = controller;
        }

        static public IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider provider)
        {
            return new[] { typeof(NodeReport), typeof(SwitchBinaryReport) };
        }

        public async Task<object> Invoke(string nodeID, string commandClassName, string operationName)
        {
            var queryParameters = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters;

            var nodes = await Controller.GetNodes().ConfigureAwait(false);
            var node = nodes[byte.Parse(nodeID)];
            if (node == null)
                return Task.FromResult<object>(null);

            var commandClassType = typeof(CommandClassBase).Assembly.GetExportedTypes().FirstOrDefault(element => element.IsSubclassOf(typeof(CommandClassBase)) && string.Compare(element.Name, commandClassName, true) == 0);
            if (commandClassType == null)
                return Task.FromResult<object>(null);

            var getCommandClassMethod = typeof(Node).GetMethod(nameof(Node.GetCommandClass)).MakeGenericMethod(commandClassType);
            if (getCommandClassMethod == null)
                return Task.FromResult<object>(null);

            var commandClass = getCommandClassMethod.Invoke(node, null);
            var invokeMethod = commandClass.GetType().GetMethods().FirstOrDefault(element => string.Compare(element.Name, operationName, true) == 0);

            var argumentValues = new List<object>();
            foreach (var parameter in invokeMethod.GetParameters())
            {
                var queryParameterName = queryParameters.Keys.Cast<string>().FirstOrDefault(element => string.Compare(element, parameter.Name, true) == 0);
                if (queryParameterName != null)
                {
                    var queryParameterValue = queryParameters[queryParameterName];
                    var argumentValue = Convert.ChangeType(queryParameterValue, parameter.ParameterType, CultureInfo.InstalledUICulture);
                    argumentValues.Add(argumentValue);
                }
            }

            var returnValue = invokeMethod.Invoke(commandClass, argumentValues.ToArray());
            if (returnValue is Task)
            {
                await ((Task)returnValue).ConfigureAwait(false);

                if (invokeMethod.ReturnType.IsGenericType)
                {
                    var resultProperty = returnValue.GetType().GetProperty(nameof(Task<object>.Result));
                    var resultValue = resultProperty.GetValue(returnValue);
                    return await Task.FromResult(resultValue.ToString()).ConfigureAwait(false);
                }
            }

            return await Task.FromResult<object>(null).ConfigureAwait(false);
        }
    }
}

#endif