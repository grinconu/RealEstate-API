using RealEstate.Domain.DTOs.Base;

namespace RealEstate.Domain.DTOs.Property;

public class PropertyPriceUpdateModel : IPersistenceUpdate<Guid>
{
    public Guid Id { get; set; }
    
    public decimal Price { get; set; }
}

public class PropertyUpdateModel : IPersistenceUpdate<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Year { get; set; }
    public Guid OwnerId { get; set; }
}