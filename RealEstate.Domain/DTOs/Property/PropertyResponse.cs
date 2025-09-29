namespace RealEstate.Domain.DTOs.Property;

public class PropertyResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string InternalCode { get; set; } = default!;
    public int Year { get; set; }
    public double Price { get; set; }
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = default!;
    public IEnumerable<PropertyImageResponse> Images { get; set; } = [];
}

public class PropertyImageResponse
{
    public Guid Id { get; set; }  
    public string Url { get; set; } = default!;
    public bool Enabled { get; set; }
}