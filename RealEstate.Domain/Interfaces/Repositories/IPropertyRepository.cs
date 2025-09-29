using System.Linq.Expressions;
using RealEstate.Domain.DTOs.Property;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces.Repositories.Base;
using RealEstate.Shared.Entities;
using RealEstate.Shared.Entities.Response;

namespace RealEstate.Domain.Interfaces.Repositories;

public interface IPropertyRepository : IBaseRepository<Property>
{
    Task<Result<PagedResult<PropertyResponse>>> GetPagedAsync(ListPropertyFiltersRequest request);
}