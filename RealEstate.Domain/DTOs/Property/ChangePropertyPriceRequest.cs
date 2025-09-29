using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RealEstate.Domain.DTOs.Property;

public class ChangePropertyPriceRequest
{
    [JsonIgnore]
    public Guid PropertyId { get; set; }

    [Range(0.1, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
    public decimal NewPrice { get; set; }
}