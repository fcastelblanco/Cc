using System;
using System.Collections;
using System.Collections.Generic;

namespace Isn.Common.CustomResponse
{
    public class ResponseWrapper<T> 
    {
        public T Data { get; set; }
        public Exception Exception { get; set; }
    }
}