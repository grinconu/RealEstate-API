using RealEstate.Domain.DTOs.Base;

namespace RealEstate.Domain.DTOs.Image;

public class PropertyImageUpdateModel : IPersistenceUpdate<Guid>
{
    public Guid Id { get; set; }
    public bool Enabled { get; set; }
}