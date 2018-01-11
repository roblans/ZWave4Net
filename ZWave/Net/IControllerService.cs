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
    interface IControllerService
    {
        [WebInvoke(Method = "GET", UriTemplate = "controller/nodes/{node}/classes/{class}/methods/{method}/*", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [ServiceKnownType(nameof(ControllerService.GetKnownTypes), typeof(ControllerService))]
        Task<object> Invoke(string node, string @class, string method);
    }
}

#endif