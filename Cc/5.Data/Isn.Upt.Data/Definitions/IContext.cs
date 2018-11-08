using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Isn.Upt.Domain;

namespace Isn.Upt.Data.Definitions
{
    public interface IContext
    {
        IDbSet<User> Users { get; set; }
        IDbSet<Company> Companies { get; set; }
        IDbSet<CompanyUpdate> CompanyUpdates { get; set; }
        IDbSet<Release> Releases { get; set; }
        IDbSet<UnhandledException> UnhandledExceptions { get; set; }
        IDbSet<CompanyModule> CompanyModules { get; set; }
        IDbSet<Module> Modules { get; set; }
        IDbSet<Server> Servers { get; set; }
        IDbSet<CompanyRelease> CompanyRelease { get; set; }
        IDbSet<UserToken> UserTokens { get; set; }
        IDbSet<Parameter> Parameters { get; set; }
        IDbSet<DownloadRequestRelease> DownloadRequestReleases { get; set; }
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        int SaveChanges();
    }
}
