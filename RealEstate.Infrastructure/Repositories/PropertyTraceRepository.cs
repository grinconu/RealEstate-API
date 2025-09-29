using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces.Repositories;
using RealEstate.Infrastructure.Context;
using RealEstate.Infrastructure.Repositories.Base;

namespace RealEstate.Infrastructure.Repositories;

public class PropertyTraceRepository(RealEstateDbContext context) : BaseRepository<PropertyTrace>(context), IPropertyTraceRepository;