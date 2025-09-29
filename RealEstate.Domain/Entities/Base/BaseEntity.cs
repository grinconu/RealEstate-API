using System.ComponentModel.DataAnnotations;

namespace RealEstate.Domain.Entities.Base;

public class BaseEntity<TKey> where TKey : IEquatable<TKey>
{
    [Key]
    public TKey Id { get; set; }
}

public class BaseEntity : BaseEntity<Guid>;