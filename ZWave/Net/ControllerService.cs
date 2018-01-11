#if NET
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using ZWave.CommandClasses;

namespace ZWave.Net
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class ControllerService : IControllerService
    {
        public readonly ZWaveController Controller;

        public ControllerService(ZWaveController controller)
        {
            Controller = controller;
        }

        static public IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider provider)
        {
            return new[] { typeof(NodeReport) };
        }

        public Task<object> Invoke(string node, string @class, string method)
        {
            var parameters = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters;

            //var x = Convert.ChangeType(parameters["a"], typeof(bool));

            WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.OK;

            return Task.FromResult<object>("Test");
        }
    }
}

#endif