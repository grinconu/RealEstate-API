using System.Net;
using Microsoft.Extensions.Logging;
using RealEstate.Domain.Constants;
using RealEstate.Domain.DTOs.Image;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces.ExternalServices;
using RealEstate.Domain.Interfaces.Services;
using RealEstate.Domain.Utilities;
using RealEstate.Shared.Entities;
using RealEstate.Shared.Entities.Response;

namespace RealEstate.Application.Implements.UseCases;

public class AddImageToPropertyUseCase(
    ILogger<AddImageToPropertyUseCase> logger,
    IPropertyService propertyService,
    IPropertyImageService propertyImageService,
    IFileStorageService fileStorageService
)
{
    public async Task<Result<Unit>> Execute(AddImageToPropertyRequest request)
    {
        string? finalFileName = null;
        var uploadSucceeded = false;
        try
        {
            var property = await propertyService.AnyAsync(p => p.Id == request.PropertyId);
            if (!property.Success || !property.Value)
            {
                logger.LogError("Not Found Property: {id}", request.PropertyId);
                return Result.BadRequest<Unit>(GenericErrors.BadRequest);
            }

            var extension = Path.GetExtension(request.FileName);
            if (!extension.IsValidImageExtension())
            {
                logger.LogError("Invalid file extension {ext} for Property {id}", extension, request.PropertyId);
                return Result.BadRequest<Unit>("Invalid image format");
            }

            var imageId = Guid.NewGuid();
            finalFileName = $"{imageId}{extension}";
            var resultUpload = await fileStorageService.UploadImageAsync(request.PropertyId.ToString(), finalFileName,
                request.File.OpenReadStream());
            if (!resultUpload.Success || string.IsNullOrEmpty(resultUpload.Value))
            {
                logger.LogError("Error uploading image for Property {id}", request.PropertyId);
                return Result.Failure([Error.Create("Error saving image")], HttpStatusCode.InternalServerError);
            }
            
            uploadSucceeded = true;

            var resultInsert = await propertyImageService.InsertAsync(new PropertyImage
            {
                Id = imageId,
                PropertyId = request.PropertyId,
                File = resultUpload.Value
            });
            if (!resultInsert.Success || resultInsert.Value == Guid.Empty)
            {
                logger.LogError("Error saving image record for Property {id}", request.PropertyId);
                await fileStorageService.DeleteImageAsync(request.PropertyId.ToString(), finalFileName);
                return Result.Failure([Error.Create("Error saving image record")], HttpStatusCode.InternalServerError);
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in AddImageToPropertyUseCase for Property {id}", request.PropertyId);
            if (!uploadSucceeded || string.IsNullOrEmpty(finalFileName))
                return Result.Failure([Error.Create("Internal server error")], HttpStatusCode.InternalServerError);
            
            try
            {
                await fileStorageService.DeleteImageAsync(request.PropertyId.ToString(), finalFileName);
            }
            catch (Exception deleteEx)
            {
                logger.LogError(deleteEx, "Failed to delete image in rollback for Property {id}", request.PropertyId);
            }
            
            return Result.Failure([Error.Create("Internal server error")], HttpStatusCode.InternalServerError);
        }
    }
}