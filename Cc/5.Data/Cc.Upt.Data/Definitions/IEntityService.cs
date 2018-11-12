using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Cc.Common.Implementations;

namespace Cc.Upt.Data.Definitions
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