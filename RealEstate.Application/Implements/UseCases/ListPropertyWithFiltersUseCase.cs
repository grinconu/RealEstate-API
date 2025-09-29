using System.Net;
using Microsoft.Extensions.Logging;
using RealEstate.Domain.DTOs.Property;
using RealEstate.Domain.Interfaces.Services;
using RealEstate.Shared.Entities;
using RealEstate.Shared.Entities.Response;

namespace RealEstate.Application.Implements.UseCases;

public class ListPropertyWithFiltersUseCase(
    ILogger<ListPropertyWithFiltersUseCase> logger,
    IPropertyService propertyService
)
{
    public async Task<Result<PagedResult<PropertyResponse>>> Execute(ListPropertyFiltersRequest request)
    {
        try
        {
            var result = await propertyService.GetPagedAsync(request);

            if (!result.Success)
            {
                logger.LogError("Error fetching properties with filters");
                return Result.Failure<PagedResult<PropertyResponse>>(
                    [Error.Create("Error fetching properties")],
                    HttpStatusCode.InternalServerError
                );
            }

            return Result.Success(result.Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in ListPropertyWithFiltersUseCase");
            return Result.Failure<PagedResult<PropertyResponse>>(
                [Error.Create("Internal server error")],
                HttpStatusCode.InternalServerError
            );
        }
    }
}