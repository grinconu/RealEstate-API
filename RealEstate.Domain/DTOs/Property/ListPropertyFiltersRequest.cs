using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RealEstate.Domain.DTOs.Property;

public class ListPropertyFiltersRequest
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? CodeInternal { get; set; }
    public int? Year { get; set; }
    public Guid? OwnerId { get; set; }
    public string? OwnerName { get; set; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; set; }
    [JsonIgnore]
    public int PageSize { get; set; } = 10;
}