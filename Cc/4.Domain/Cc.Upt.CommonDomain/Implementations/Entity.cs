using System.ComponentModel.DataAnnotations;
using Cc.Upt.CommonDomain.Definitions;

namespace Cc.Upt.CommonDomain.Implementations
{
    public abstract class Entity<T> : IEntity<T>
    {
        [Key]
        public virtual T Id { get; set; }

    }
}
