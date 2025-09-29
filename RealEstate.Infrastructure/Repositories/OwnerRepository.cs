using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces.Repositories;
using RealEstate.Infrastructure.Context;
using RealEstate.Infrastructure.Repositories.Base;

namespace RealEstate.Infrastructure.Repositories;

public class OwnerRepository(RealEstateDbContext context) : BaseRepository<Owner>(context), IOwnerRepository;