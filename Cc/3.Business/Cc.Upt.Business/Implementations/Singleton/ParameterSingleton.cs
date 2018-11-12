using System.Collections.Generic;
using Cc.Upt.Common.Implementations;
using Cc.Upt.Domain;

namespace Cc.Upt.Business.Implementations.Singleton
{
    public class ParameterSingleton : HelperSingleton<ParameterSingleton>
    {
        public List<Parameter> ParameterList { get; set; }
    }
}