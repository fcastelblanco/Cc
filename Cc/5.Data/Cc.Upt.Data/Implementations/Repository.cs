using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Cc.Upt.Data.Definitions;
using Cc.Upt.Domain.Repository.Definitions;

namespace Cc.Upt.Data.Implementations
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly IContext Context;
        protected IDbSet<T> Dbset;

        protected Repository(IContext context)
        {
            Context = context;
            Dbset = Context.Set<T>();
        }
        
        public virtual void Create(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Context.Entry(entity).State = EntityState.Added;
            Context.SaveChanges();
        }
        
        public virtual void Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            Context.Entry(entity).State = EntityState.Modified;
            Context.SaveChanges();
        }

        public virtual void Delete(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            Context.Entry(entity).State = EntityState.Detached;
            Context.SaveChanges();
        }

        public virtual IEnumerable<T> GetAll()
        {
            return Dbset.AsEnumerable();
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            var query = Dbset.Where(predicate);
            return query;
        }
    }
}