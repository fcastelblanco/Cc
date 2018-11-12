using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Data.Definitions;
using Cc.Upt.Data.Implementations;
using Cc.Upt.Domain;

namespace Cc.Upt.Business.Implementations
{
    public class CompanyService : Repository<Company>, ICompanyService
    {
        public CompanyService(IContext context) : base(context)
        {
            Dbset = context.Set<Company>();
        }

        public IEnumerable<Company> GetAllCompanys()
        {
            return GetAll();
        }

        public Company GetById(Guid id)
        {
            return Dbset.FirstOrDefault(x => x.Id == id);
        }

        public Company GetByName(string name)
        {
            var currentLowerName = name.ToLower();
            return Dbset.FirstOrDefault(x => x.Name.ToLower() == currentLowerName);
        }

        public bool Save(Company model)
        {
            var exists = Dbset.FirstOrDefault(x => x.Id == model.Id);
            if (exists != null)
            {
                exists.DateEndSupport = model.DateEndSupport;
                exists.Name = model.Name;
                Update(exists);
            }
            else
                Create(model);

            return true;
        }

        public IEnumerable<Company> GetCompanyListByPeriodSupportOpen()
        {
            var currentDate = DateTime.Now.Date;
            return FindBy(x => DbFunctions.TruncateTime( x.DateEndSupport) >= currentDate).ToList();
        }
    }
}