using System.Linq.Expressions;
using MapsterMapper;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces.Repositories;
using RealEstate.Domain.Interfaces.Services;
using RealEstate.Application.Implements.Services.Base;
using RealEstate.Domain.DTOs.Property;
using RealEstate.Shared.Entities;
using RealEstate.Shared.Entities.Response;

namespace RealEstate.Application.Implements.Services;

public class PropertyService(
    IPropertyRepository repository,
    IMapper mapper
    ) : BaseService<Property>(repository, mapper), IPropertyService
{
    public async Task<Result<PagedResult<PropertyResponse>>> GetPagedAsync(ListPropertyFiltersRequest request)
    {
        return await repository.GetPagedAsync(request);
    }
}