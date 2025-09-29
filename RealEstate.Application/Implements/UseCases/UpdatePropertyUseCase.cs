using System.Net;
using Microsoft.Extensions.Logging;
using RealEstate.Domain.Constants;
using RealEstate.Domain.DTOs.Image;
using RealEstate.Domain.DTOs.Property;
using RealEstate.Domain.Interfaces.Services;
using RealEstate.Domain.Interfaces.Utilities;
using RealEstate.Shared.Entities;
using RealEstate.Shared.Entities.Response;

namespace RealEstate.Application.Implements.UseCases;

public class UpdatePropertyUseCase(
    ILogger<UpdatePropertyUseCase> logger,
    IPropertyService propertyService,
    IPropertyImageService propertyImageService,
    IUnitOfWork unitOfWork
)
{
    public async Task<Result<Unit>> Execute(UpdatePropertyRequest request)
    {
        try
        {
            var property = await propertyService.AnyAsync(p => p.Id == request.Id);
            if (!property.Success || !property.Value)
            {
                logger.LogError("Not Found Property: {Id}", request.Id);
                return Result.BadRequest<Unit>(GenericErrors.BadRequest);
            }

            await unitOfWork.BeginTransactionAsync();
            
            var resultUpdate = await propertyService.UpdateTransactAsync(new PropertyUpdateModel
            {
                Id = request.Id,
                Name = request.Name,
                Address = request.Address,
                Year = request.Year,
                OwnerId = request.OwnerId
            });

            if (!resultUpdate.Success)
            {
                logger.LogError("Error updating Property {Id}", request.Id);
                await unitOfWork.RollbackAsync();
                return Result.Failure<Unit>(
                    [Error.Create("Error updating property")],
                    HttpStatusCode.InternalServerError
                );
            }
            
            if (request.Images.Any())
            {
                foreach (var image in request.Images)
                {
                    var resultImageUpdate = await propertyImageService.UpdateTransactAsync(new PropertyImageUpdateModel
                    {
                        Id = image.Id,
                        Enabled = image.IsEnabled,
                    });
                    if (!resultImageUpdate.Success)
                    {
                        logger.LogError("Error updating status of Image {ImageId} for Property {Id}", image.Id, request.Id);
                        await unitOfWork.RollbackAsync();
                        return Result.Failure<Unit>(
                            [Error.Create("Error updating property images")],
                            HttpStatusCode.InternalServerError
                        );
                    }
                }
            }
            
            await unitOfWork.CommitAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();
            logger.LogError(ex, "Error in UpdatePropertyUseCase for Property {Id}", request.Id);
            return Result.Failure<Unit>(
                [Error.Create("Internal server error")],
                HttpStatusCode.InternalServerError
            );
        }
    }
}