using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Isn.Upt.Business.Definitions;
using Isn.Upt.Data.Definitions;
using Isn.Upt.Data.Implementations;
using Isn.Upt.Domain;

namespace Isn.Upt.Business.Implementations
{
    public class CompanyService : EntityService<Company>, ICompanyService
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