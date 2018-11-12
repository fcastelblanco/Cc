using System;
using System.Collections.Generic;
using Cc.Upt.Data.Definitions;
using Cc.Upt.Domain;
using Cc.Upt.Domain.Dto;
using Cc.Upt.Domain.Enumerations;
using Cc.Upt.DomainRepository.Definitions;

namespace Cc.Upt.Business.Definitions
{
    public interface IParameterService : IRepository<Parameter>
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