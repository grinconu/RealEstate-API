using System.Net;
using Microsoft.Extensions.Logging;
using RealEstate.Domain.Constants;
using RealEstate.Domain.DTOs.Property;
using RealEstate.Domain.DTOs.PropertyTrace;
using RealEstate.Domain.Interfaces.Services;
using RealEstate.Domain.Interfaces.Utilities;
using RealEstate.Shared.Entities;
using RealEstate.Shared.Entities.Response;

namespace RealEstate.Application.Implements.UseCases;

public class ChangePropertyPriceUseCase(
    ILogger<ChangePropertyPriceUseCase> logger,
    IPropertyService propertyService,
    IPropertyTraceService propertyTraceService,
    IUnitOfWork unitOfWork
)
{
    public async Task<Result<Unit>> Execute(ChangePropertyPriceRequest request)
    {
        try
        {
            var property = await propertyService.AnyAsync(p => p.Id == request.PropertyId);
            if (!property.Success || !property.Value)
            {
                logger.LogError("Not Found Property: {Id}", request.PropertyId);
                return Result.BadRequest<Unit>(GenericErrors.BadRequest);
            }

            await unitOfWork.BeginTransactionAsync();

            var resultUpdate = await propertyService.UpdateTransactAsync(new PropertyPriceUpdateModel
            {
                Id = request.PropertyId,
                Price = request.NewPrice
            });
            if (!resultUpdate.Success)
            {
                logger.LogError("Error updating price for Property {Id}", request.PropertyId);
                return Result.Failure<Unit>([Error.Create("Error updating price")], HttpStatusCode.InternalServerError);
            }

            var resultTrace = await propertyTraceService.InsertTransactAsync(new PropertyTraceModel
            {
                PropertyId = request.PropertyId,
                DateSale = DateTime.UtcNow,
                Name = "Price change",
                Value = request.NewPrice,
                Tax = CalculateTax(request.NewPrice)
            });

            if (!resultTrace.Success)
            {
                await unitOfWork.RollbackAsync();
                logger.LogError("Error inserting trace for price change on Property {Id}", request.PropertyId);
                return Result.Failure<Unit>([Error.Create("Error updating price")], HttpStatusCode.InternalServerError);
            }

            await unitOfWork.CommitAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();
            logger.LogError(ex, "Error in ChangePriceUseCase for Property {Id}", request.PropertyId);
            return Result.Failure<Unit>([Error.Create("Internal server error")], HttpStatusCode.InternalServerError);
        }
    }

    private decimal CalculateTax(decimal price)
    {
        return 0m;
    }
}