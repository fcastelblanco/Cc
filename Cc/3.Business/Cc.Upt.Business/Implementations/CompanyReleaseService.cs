using System;
using System.Collections.Generic;
using System.Linq;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Data.Definitions;
using Cc.Upt.Data.Implementations;
using Cc.Upt.Domain;

namespace Cc.Upt.Business.Implementations
{
    public class CompanyReleaseService : Repository<ServerRelease>, ICompanyReleaseService
    {
        public CompanyReleaseService(IContext context) : base(context)
        {
            Dbset = context.Set<ServerRelease>();
        }

        public bool Save(ServerRelease model)
        {
            try
            {
                Create(model);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public List<ServerRelease> GetCompanyReleaseList(Guid companyId)
        {
            return FindBy(x => x.ServerId == companyId).ToList();
        }


        public List<Guid> GetReleaseListDownload(IEnumerable<Guid> releaselist, Guid companyId)
        {
            return FindBy(x => releaselist.Contains(x.ReleaseId) && x.ServerId != companyId).Select(x => x.ReleaseId).ToList();
        }

        public IEnumerable<ServerRelease> GetCompanyReleaseListByReleaseId(Guid releaseId)
        {
            return FindBy(x => x.ReleaseId == releaseId).ToList();
        }
    }
}