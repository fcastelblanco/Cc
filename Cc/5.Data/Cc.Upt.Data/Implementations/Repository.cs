using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Cc.Upt.Data.Definitions;
using Cc.Upt.DomainRepository.Definitions;

namespace Cc.Upt.Data.Implementations
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly IContext _context;
        protected IDbSet<T> Dbset;

        protected Repository(IContext context)
        {
            _context = context;
            Dbset = _context.Set<T>();
        }
        
        public virtual void Create(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Entry(entity).State = EntityState.Added;
            _context.SaveChanges();
        }
        
        public virtual void Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public virtual void Delete(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            _context.Entry(entity).State = EntityState.Detached;
            _context.SaveChanges();
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