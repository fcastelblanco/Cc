﻿using System;
using System.Collections.Generic;
using System.Linq;
using Isn.Upt.Business.Definitions;
using Isn.Upt.Data.Definitions;
using Isn.Upt.Data.Implementations;
using Isn.Upt.Domain;

namespace Isn.Upt.Business.Implementations
{
    public class CompanyReleaseService : EntityService<CompanyRelease>, ICompanyReleaseService
    {
        public CompanyReleaseService(IContext context) : base(context)
        {
            Dbset = context.Set<CompanyRelease>();
        }

        public bool Save(CompanyRelease model)
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

        public List<CompanyRelease> GetCompanyReleaseList(Guid companyId)
        {
            return FindBy(x => x.CompanyId == companyId).ToList();
        }


        public List<Guid> GetReleaseListDownload(IEnumerable<Guid> releaselist, Guid companyId)
        {
            return FindBy(x => releaselist.Contains(x.ReleaseId) && x.CompanyId != companyId).Select(x => x.ReleaseId).ToList();
        }

        public IEnumerable<CompanyRelease> GetCompanyReleaseListByReleaseId(Guid releaseId)
        {
            return FindBy(x => x.ReleaseId == releaseId).ToList();
        }
    }
}