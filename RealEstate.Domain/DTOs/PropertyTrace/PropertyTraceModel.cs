namespace RealEstate.Domain.DTOs.PropertyTrace;

public class PropertyTraceModel
{
    public DateTime DateSale { get; set; }
    public string Name { get; set; }
    public decimal Value { get; set; }
    public decimal Tax { get; set; }
    public Guid PropertyId { get; set; }
}