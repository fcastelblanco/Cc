using Isn.Upt.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isn.Upt.Business.Definitions
{
    public interface IUpdaterService
    {
        void Execute();
        T GetConfigurationFromXml<T>( string path); 
    }
}
