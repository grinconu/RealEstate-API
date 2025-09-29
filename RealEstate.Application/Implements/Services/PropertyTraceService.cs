using MapsterMapper;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces.Repositories;
using RealEstate.Domain.Interfaces.Services;
using RealEstate.Application.Implements.Services.Base;

namespace RealEstate.Application.Implements.Services;

public class PropertyTraceService(
    IPropertyTraceRepository repository,
    IMapper mapper
    ) : BaseService<PropertyTrace>(repository, mapper), IPropertyTraceService;