using System.ComponentModel.DataAnnotations;
using Cc.Common.Definitions;

namespace Cc.Common.Implementations
{
    public abstract class Entity<T> : BaseEntity, IEntity<T>
    {
        [Key]
        public virtual T Id { get; set; }

    }
}
