using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Entities.Base;

namespace RealEstate.Domain.Entities;

public class PropertyTrace : BaseEntity
{
    public DateTime DateSale { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Value { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Tax { get; set; }

    [ForeignKey("Property")]
    public Guid PropertyId { get; set; }
    public Property Property { get; set; }
}