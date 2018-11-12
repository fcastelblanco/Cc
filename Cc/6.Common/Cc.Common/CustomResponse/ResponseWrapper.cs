using System;

namespace Cc.Common.CustomResponse
{
    public class ResponseWrapper<T> 
    {
        public T Data { get; set; }
        public Exception Exception { get; set; }
    }
}