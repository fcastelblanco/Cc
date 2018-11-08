using System;
using Isn.Common.Implementations;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Isn.Upt.Data.Definitions
{
    public interface IEntityService<T> : IService
        where T : BaseEntity
    {
        void Create(T entity);
        void Delete(T entity);
        IEnumerable<T> GetAll();
        void Update(T entity);
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
    }
}