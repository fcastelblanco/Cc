using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Threading;
using Cc.Common.Definitions;
using Cc.Upt.Data.Definitions;
using Cc.Upt.Domain;

namespace Cc.Upt.Data.Implementations
{
    public class Context : DbContext, IContext
    {
        public Context()
            : base("Upt")
        {
            Configuration.LazyLoadingEnabled = false;
            Database.SetInitializer<Context>(null);
        }

        public IDbSet<User> Users { get; set; }
        public IDbSet<Company> Companies { get; set; }
        public IDbSet<CompanyUpdate> CompanyUpdates { get; set; }
        public IDbSet<Release> Releases { get; set; }
        public IDbSet<UnhandledException> UnhandledExceptions { get; set; }
        public IDbSet<CompanyModule> CompanyModules { get; set; }
        public IDbSet<Module> Modules { get; set; }
        public IDbSet<Server> Servers { get; set; }
        public IDbSet<CompanyRelease> CompanyRelease { get; set; }

        public override int SaveChanges()
        {
            var modifiedEntries = ChangeTracker.Entries()
                .Where(x => x.Entity is IAuditableEntity
                            && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entry in modifiedEntries)
            {
                if (!(entry.Entity is IAuditableEntity entity)) continue;

                var identityName = Thread.CurrentPrincipal.Identity.Name;
                var now = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedBy = identityName;
                    entity.CreatedDate = now;
                }
                else
                {
                    Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                    Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                    entity.UpdatedBy = identityName;
                    entity.UpdatedDate = now;
                }
            }

            return base.SaveChanges();
        }

        public IDbSet<UserToken> UserTokens { get; set; }

        public IDbSet<Parameter> Parameters { get; set; }
        public IDbSet<DownloadRequestRelease> DownloadRequestReleases { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}