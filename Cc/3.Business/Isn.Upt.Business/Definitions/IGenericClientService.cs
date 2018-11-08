using System.Collections.Generic;
using System.Threading.Tasks;
using Isn.Common.CustomResponse;

namespace Isn.Upt.Business.Definitions
{
    public interface IGenericClientService
    {
        ResponseWrapper<T> Get<T>(string requestUri, IDictionary<string, string> aditionalHeaders = null);
        ResponseWrapper<T> Post<T>(object entity, string requestUri, IDictionary<string, string> aditionalHeaders = null);
    }
}