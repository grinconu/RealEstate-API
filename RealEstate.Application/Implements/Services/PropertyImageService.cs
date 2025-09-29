using MapsterMapper;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces.Repositories;
using RealEstate.Domain.Interfaces.Services;
using RealEstate.Application.Implements.Services.Base;

namespace RealEstate.Application.Implements.Services;

public class PropertyImageService(
    IPropertyImageRepository repository,
    IMapper mapper
    ) : BaseService<PropertyImage>(repository, mapper), IPropertyImageService;