using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Entities.Base;

namespace RealEstate.Domain.Entities;

public class PropertyImage : BaseEntity
{
    [Required]
    public string File { get; set; }

    public bool Enabled { get; set; } = true;

    [ForeignKey("Property")]
    public Guid PropertyId { get; set; }
    public Property Property { get; set; }
}