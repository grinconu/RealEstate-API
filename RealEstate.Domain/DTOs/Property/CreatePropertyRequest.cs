using System.ComponentModel.DataAnnotations;

namespace RealEstate.Domain.DTOs.Property;

public class CreatePropertyRequest
{
    [Required, MaxLength(200)]
    public string Name { get; set; }
    
    [Required, MaxLength(500)]
    public string Address { get; set; }
    
    [Required, MaxLength(50)]
    public string CodeInternal { get; set; }
    
    [Required]
    public decimal Price { get; set; }
    
    [Required]
    public int Year { get; set; }
    
    [Required]
    public Guid OwnerId { get; set; }
}