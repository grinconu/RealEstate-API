using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces.Repositories;
using RealEstate.Infrastructure.Context;
using RealEstate.Infrastructure.Repositories.Base;

namespace RealEstate.Infrastructure.Repositories;

public class PropertyImageRepository(RealEstateDbContext context) : BaseRepository<PropertyImage>(context), IPropertyImageRepository;