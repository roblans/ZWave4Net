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
using System.IO;
using System.Text;
using System.ServiceModel.Channels;

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

        public async Task<Message> Invoke(string nodeID, string commandClassName, string operationName)
        {
            var context = WebOperationContext.Current;

            // Example: http://localhost:80/api/v1.0/controller/nodes/19/switchbinary/set/?value=true
            // nodeID = 19, commandClassName = switchbinary, operationName = set, query = "value=true"

            // get query: ["value"] = "true"
            var queryParameters = context.IncomingRequest.UriTemplateMatch.QueryParameters;

            // we need to execute: Controller.GetNodes()[nodeID].GetCommandClass<commandClassName>().operationName(p1, p2, p3, ...);
            // so lets have fun with reflection

            // get all the nodes
            var nodes = await Controller.GetNodes();

            // get the node
            var node = nodes[byte.Parse(nodeID)];
            if (node == null)
                return CreateErrorResponse(context, $"Node: {nodeID} not found.");


            // get the commandclasstype (case insensitive match)
            var commandClassType = typeof(CommandClassBase).Assembly.GetExportedTypes().FirstOrDefault(element => element.IsSubclassOf(typeof(CommandClassBase)) && string.Compare(element.Name, commandClassName, true) == 0);
            if (commandClassType == null)
                return CreateErrorResponse(context, $"CommandClass: {commandClassName} not found.");

            // get the method: GetCommandClass<commandClassName>()
            var getCommandClassMethod = typeof(Node).GetMethod(nameof(Node.GetCommandClass)).MakeGenericMethod(commandClassType);
            if (getCommandClassMethod == null)
                return CreateErrorResponse(context, $"Node: {nodeID} does not support CommandClass: {commandClassName}.");

            // invoke method to get the commandclass instance 
            var commandClass = getCommandClassMethod.Invoke(node, null);

            // get the commandClassName.operationName method
            var invokeMethod = commandClass.GetType().GetMethods().FirstOrDefault(element => string.Compare(element.Name, operationName, true) == 0);

            // process arguments
            var argumentValues = new List<object>();

            // get the parameters of the method
            foreach (var parameter in invokeMethod.GetParameters())
            {
                // find matching parameter in querey
                var queryParameterName = queryParameters.Keys.Cast<string>().FirstOrDefault(element => string.Compare(element, parameter.Name, true) == 0);

                // found?
                if (queryParameterName != null)
                {
                    // yes, so get value
                    var queryParameterValue = queryParameters[queryParameterName];

                    // is it a enum?
                    if (parameter.ParameterType.IsEnum)
                    {
                        // yes, so convert string to enum
                        var enumValue = Enum.Parse(parameter.ParameterType, queryParameterValue);
                        
                        // and add typed parameter to collection 
                        argumentValues.Add(enumValue);

                        // done
                        continue;
                    }

                    // convert string to typed parameter 
                    var argumentValue = Convert.ChangeType(queryParameterValue, parameter.ParameterType, CultureInfo.InvariantCulture);
                    
                    // and add typed parameter to collection 
                    argumentValues.Add(argumentValue);
                }
            }

            // invoke het commandclass method: commandClass.operationName(p1, p2, p3, ...) 
            var returnValue = invokeMethod.Invoke(commandClass, argumentValues.ToArray());

            // do we have a return value
            if (returnValue != null)
            {
                // is the return value a Task
                if (returnValue is Task)
                {
                    // yes, so await for the task to complete
                    await ((Task)returnValue);

                    // if the task is a generic Task<T> then the task has a result
                    if (invokeMethod.ReturnType.IsGenericType)
                    {
                        // get the result property
                        var resultProperty = returnValue.GetType().GetProperty(nameof(Task<object>.Result));
                        // get the result value
                        var resultValue = resultProperty.GetValue(returnValue);

                        // return response
                        return CreateValidResponse(context, resultValue);
                    }

                    // return response
                    return CreateNullResponse(context);
                }

                // return response
                return CreateValidResponse(context, returnValue);
            }

            return CreateNullResponse(context);
        }

        private Message CreateValidResponse(WebOperationContext context, object value)
        {
            switch (context.IncomingRequest.Accept)
            {
                case "application/xml":
                    return context.CreateXmlResponse(value.ToString());
                case "application/json":
                    return context.CreateJsonResponse(value.ToString());
                default:
                    return context.CreateTextResponse(value.ToString());
            }
        }

        private Message CreateNullResponse(WebOperationContext context)
        {
            context.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.NoContent;
            return null;
        }

        private Message CreateErrorResponse(WebOperationContext context, string error)
        {
            return context.CreateTextResponse(error);
        }
    }
}

#endif