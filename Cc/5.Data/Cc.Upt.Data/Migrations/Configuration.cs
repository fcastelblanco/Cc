using System;
using System.Data.Entity.Migrations;
using Cc.Upt.Common.ExtensionMethods;
using Cc.Upt.Data.Implementations;
using Cc.Upt.Domain;
using Cc.Upt.Domain.Enumerations;

namespace Cc.Upt.Data.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Context context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            var company = new Company
            {
                Id = Guid.Parse("3C3DE7C2-3D98-4C93-87B1-9176A76C40AE"),
                Email = "contacto@updater.com",
                Mobile = "3102017739",
                Name = "Updater",
                Phone = "8012108",
                TaxId = "1020717498-6",
                Url = "http://www.updater.com"
            };

            context.Companies.AddOrUpdate(company);

            var user = new User
            {
                Id = Guid.Parse("2CB4E82E-9DF6-4B2F-B1E2-C96976C1F64C"),
                CompanyId = company.Id,
                Email= "contacto@updater.com",
                Name = "Freddy",
                LastName  ="Castelblanco",
                Password = "admin".Encrypt(StringExtension.PassPhrase),
                Profile = Profile.Administrator                
            };

            context.Users.AddOrUpdate(user);

            context.SaveChanges();
        }
    }
}
