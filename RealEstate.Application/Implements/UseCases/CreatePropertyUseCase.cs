using System.Net;
using Microsoft.Extensions.Logging;
using RealEstate.Domain.Constants;
using RealEstate.Domain.DTOs.Property;
using RealEstate.Domain.Interfaces.Services;
using RealEstate.Shared.Entities;
using RealEstate.Shared.Entities.Response;

namespace RealEstate.Application.Implements.UseCases;

public class CreatePropertyUseCase(
    ILogger<CreatePropertyUseCase> logger,
    IOwnerService ownerService,
    IPropertyService propertyService
    )
{
    public async Task<Result<Unit>> Execute(CreatePropertyRequest request)
    {
        var owner = await ownerService.AnyAsync(o => o.Id == request.OwnerId);
        if(!owner.Success || !owner.Value)
        {
            logger.LogError("Not Found Owner: {id}", request.OwnerId);
            return Result.BadRequest<Unit>(GenericErrors.BadRequest);
        }

        var resultInsert = await propertyService.InsertAsync(request);
        
        if(!resultInsert.Success || resultInsert.Value == Guid.Empty)
        {
            logger.LogError("Error to insert Property: {Name}", request.Name);
            return Result.Failure([Error.Create("Internal Error")], HttpStatusCode.InternalServerError);
        }
        
        return Result.Success();
    }
}
    