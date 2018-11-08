using System.ComponentModel.DataAnnotations;
using Isn.Common.Definitions;

namespace Isn.Common.Implementations
{
    public abstract class Entity<T> : BaseEntity, IEntity<T>
    {
        [Key]
        public virtual T Id { get; set; }

    }
}
