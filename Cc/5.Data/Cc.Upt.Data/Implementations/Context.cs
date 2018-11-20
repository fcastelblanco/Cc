using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Threading;
using Cc.Upt.Data.Definitions;
using Cc.Upt.Domain;
using Cc.Upt.Domain.Common.Definitions;

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

        public IDbSet<ServerUpdate> ServerUpdates { get; set; }
        public IDbSet<User> Users { get; set; }
        public IDbSet<Company> Companies { get; set; }
        public IDbSet<Release> Releases { get; set; }
        public IDbSet<Server> Servers { get; set; }
        public IDbSet<ServerRelease> ServerReleases { get; set; }
        public IDbSet<UnhandledException> UnhandledExceptions { get; set; }
        public IDbSet<UserToken> UserTokens { get; set; }
        public IDbSet<Parameter> Parameters { get; set; }
        public IDbSet<DownloadRequestRelease> DownloadRequestReleases { get; set; }

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
                    entity.CreatedOn = now;
                }
                else
                {
                    Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                    Entry(entity).Property(x => x.CreatedOn).IsModified = false;
                    entity.UpdatedBy = identityName;
                    entity.UpdatedOn = now;
                }
            }

            return base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}