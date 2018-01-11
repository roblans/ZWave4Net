#if NET

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Net
{
    [ServiceContract()]
    interface IControllerRestService
    {
        [WebInvoke(Method = "GET", UriTemplate = "controller/nodes/{nodeID}/{commandClassName}/{operationName}/*", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [ServiceKnownType(nameof(ControllerRestService.GetKnownTypes), typeof(ControllerRestService))]
        Task<object> Invoke(string nodeID, string commandClassName, string operationName);
    }
}

#endif