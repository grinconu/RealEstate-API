using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.DTOs.Property;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces.Repositories;
using RealEstate.Infrastructure.Context;
using RealEstate.Infrastructure.Repositories.Base;
using RealEstate.Shared.Entities;
using RealEstate.Shared.Entities.Response;

namespace RealEstate.Infrastructure.Repositories;

public class PropertyRepository(RealEstateDbContext context) : BaseRepository<Property>(context), IPropertyRepository
{
    public async Task<Result<PagedResult<PropertyResponse>>> GetPagedAsync(ListPropertyFiltersRequest request)
    {
         var query = DbSet
             .AsNoTracking()
             .AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(request.Name))
            query = query.Where(p => p.Name.Contains(request.Name));
        
        if (!string.IsNullOrWhiteSpace(request.Address))
            query = query.Where(p => p.Address.Contains(request.Address));
        
        if (!string.IsNullOrWhiteSpace(request.CodeInternal))
            query = query.Where(p => p.CodeInternal.Contains(request.CodeInternal));
        
        if (request.Year.HasValue)
            query = query.Where(p => p.Year == request.Year.Value);
        
        if (request.OwnerId.HasValue)
            query = query.Where(p => p.OwnerId == request.OwnerId.Value);
        
        if (!string.IsNullOrWhiteSpace(request.OwnerName))
            query = query.Where(p => p.Owner.Name.Contains(request.OwnerName));
        
        var totalCount = await query.CountAsync();

        var items = await (
                from property in query
                join image in context.PropertyImage
                    on property.Id equals image.PropertyId into propertyImages
                select new PropertyResponse
                {
                    Id = property.Id,
                    Name = property.Name,
                    Address = property.Address,
                    InternalCode = property.CodeInternal,
                    Year = property.Year,
                    Price = Convert.ToDouble(property.Price),
                    OwnerId = property.OwnerId,
                    OwnerName = property.Owner.Name,
                    Images = propertyImages
                        .Select(img => new PropertyImageResponse
                        {
                            Id = img.Id,
                            Url = img.File,
                            Enabled = img.Enabled
                        })
                        .ToList()
                }
            )
            .OrderBy(p => p.Year)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PagedResult<PropertyResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}