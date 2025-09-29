using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RealEstate.Domain.DTOs.Property;

public class UpdatePropertyRequest
{
    [JsonIgnore]
    public Guid Id { get; set; } = Guid.Empty;
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Address { get; set; } = string.Empty;
    [Required]
    public int Year { get; set; }
    [Required]
    public Guid OwnerId { get; set; }
    public List<UpdatePropertyImageRequest> Images { get; set; } = new();
}

public class UpdatePropertyImageRequest
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public bool IsEnabled { get; set; }
}