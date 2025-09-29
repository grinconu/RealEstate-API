using System.Linq.Expressions;
using RealEstate.Domain.DTOs.Property;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces.Services.Base;
using RealEstate.Shared.Entities;
using RealEstate.Shared.Entities.Response;

namespace RealEstate.Domain.Interfaces.Services;

public interface IPropertyService : IBaseService<Property>
{
    Task<Result<PagedResult<PropertyResponse>>> GetPagedAsync(ListPropertyFiltersRequest request);
}