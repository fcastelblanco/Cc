using System;
using System.Collections.Generic;
using Cc.Upt.Data.Definitions;
using Cc.Upt.Domain;
using Cc.Upt.Domain.Repository.Definitions;


namespace Cc.Upt.Business.Definitions
{
    public interface ICompanyUpdateService : IRepository<ServerUpdate>
    {
        List<ServerUpdate> GetServerUpdateList(Guid companyId);
        bool Save(ServerUpdate companyUpdate);
        ServerUpdate ValidateXmlFile(string path, string userName);
        ServerUpdate GetLastUpdate(Guid companyId);
        bool CreateXml(ServerUpdate companyUpdate, string path);
        IEnumerable<Release> GetAvailableReleaseByServerId(Guid companyId);
    }
}