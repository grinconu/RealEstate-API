using MapsterMapper;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces.Repositories;
using RealEstate.Domain.Interfaces.Services;
using RealEstate.Application.Implements.Services.Base;

namespace RealEstate.Application.Implements.Services;

public class OwnerService(
    IOwnerRepository repository,
    IMapper mapper
    ) : BaseService<Owner>(repository, mapper), IOwnerService;