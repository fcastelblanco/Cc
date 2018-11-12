using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Cc.Upt.DomainRepository.Definitions
{
    public interface IRepository<T>
    {
        void Create(T entity);
        void Delete(T entity);
        IEnumerable<T> GetAll();
        void Update(T entity);
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
    }
}