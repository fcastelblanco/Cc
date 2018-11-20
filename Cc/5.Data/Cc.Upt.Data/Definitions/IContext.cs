using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Cc.Upt.Domain;

namespace Cc.Upt.Data.Definitions
{
    public interface IContext
    {
        IDbSet<Company> Companies { get; set; }
        IDbSet<DownloadRequestRelease> DownloadRequestReleases { get; set; }
        IDbSet<Parameter> Parameters { get; set; }
        IDbSet<Release> Releases { get; set; }
        IDbSet<Server> Servers { get; set; }
        IDbSet<ServerRelease> ServerReleases { get; set; }
        IDbSet<ServerUpdate> ServerUpdates { get; set; }
        IDbSet<User> Users { get; set; }
        IDbSet<UnhandledException> UnhandledExceptions { get; set; }
        IDbSet<UserToken> UserTokens { get; set; }
        

        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        int SaveChanges();
    }
}
