using System;
using System.Collections.Generic;
using System.Xml;
using Isn.Upt.Data.Definitions;
using Isn.Upt.Domain;
using Isn.Upt.Domain.Dto;
using Isn.Upt.Domain.Enumerations;

namespace Isn.Upt.Business.Definitions
{
    public interface IParameterService : IEntityService<Parameter>
    {
        IEnumerable<Parameter> GetAllParameters();
        Parameter GetById(Guid id);
        Parameter GetByInternalIdentificator(ParameterInternalIdentificator parameterInternalIdentificator);
        bool Save(Parameter parameter, bool isSingle = false);
        void PrepareData();
        T GetParameterValueByInternalIdentificator<T>(ParameterInternalIdentificator parameterInternalIdentificator);
        IDictionary<LicenseParameter, string> GetIsolucionParameterValues(IEnumerable<IsoluctionParameterDto> parameters, string licensePath);
    }
}