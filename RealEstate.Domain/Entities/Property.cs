using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Entities.Base;

namespace RealEstate.Domain.Entities;

public class Property : BaseEntity
{
    
    [Required, MaxLength(200)]
    public string Name { get; set; }

    [MaxLength(500)]
    public string Address { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Required, MaxLength(50)]
    public string CodeInternal { get; set; }

    public int Year { get; set; }

    [ForeignKey("Owner")]
    public Guid OwnerId { get; set; }
    public Owner Owner { get; set; }
}