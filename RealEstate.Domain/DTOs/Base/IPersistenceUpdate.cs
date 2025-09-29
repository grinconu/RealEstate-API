namespace RealEstate.Domain.DTOs.Base;

public interface IPersistenceUpdate<TKey> where TKey : IEquatable<TKey>
{
    TKey Id { get; set; }
}