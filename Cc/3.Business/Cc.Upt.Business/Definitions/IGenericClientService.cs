using System.Collections.Generic;
using Cc.Upt.Common.CustomResponse;

namespace Cc.Upt.Business.Definitions
{
    public interface IGenericClientService
    {
        ResponseWrapper<T> Get<T>(string requestUri, IDictionary<string, string> aditionalHeaders = null);
        ResponseWrapper<T> Post<T>(object entity, string requestUri, IDictionary<string, string> aditionalHeaders = null);
    }
}