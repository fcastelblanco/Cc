using System.Collections.Generic;
using Isn.Common.Implementations;
using Isn.Upt.Domain;

namespace Isn.Upt.Business.Implementations.Singleton
{
    public class ParameterSingleton : HelperSingleton<ParameterSingleton>
    {
        public List<Parameter> ParameterList { get; set; }
    }
}