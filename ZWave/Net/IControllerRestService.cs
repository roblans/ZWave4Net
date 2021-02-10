#if NET45

using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
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
        Task<Message> Invoke(string nodeID, string commandClassName, string operationName);
    }
}

#endif
