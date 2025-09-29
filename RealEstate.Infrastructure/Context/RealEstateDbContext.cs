using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Context;

public class RealEstateDbContext(DbContextOptions<RealEstateDbContext> options) : DbContext(options)
{
    public virtual DbSet<Owner> Owner { get; set; }
    public virtual DbSet<Property> Property { get; set; }
    public virtual DbSet<PropertyImage> PropertyImage { get; set; }
    public virtual DbSet<PropertyTrace> PropertyTrace { get; set; }
}