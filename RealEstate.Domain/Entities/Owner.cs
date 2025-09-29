using System.ComponentModel.DataAnnotations;
using RealEstate.Domain.Entities.Base;

namespace RealEstate.Domain.Entities;

public class Owner : BaseEntity
{
    [Required, MaxLength(200)]
    public string Name { get; set; }

    [MaxLength(500)]
    public string Address { get; set; }

    public string Photo { get; set; }

    public DateTime? Birthday { get; set; }
}