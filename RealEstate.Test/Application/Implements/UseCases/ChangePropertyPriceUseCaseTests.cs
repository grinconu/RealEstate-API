using System.Linq.Expressions;
using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RealEstate.Domain.DTOs.Property;
using RealEstate.Domain.DTOs.PropertyTrace;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces.Services;
using RealEstate.Domain.Interfaces.Utilities;
using RealEstate.Application.Implements.UseCases;
using RealEstate.Shared.Entities;
using RealEstate.Shared.Entities.Response;

namespace RealEstate.Application.Test.Application.Implements.UseCases;

[TestFixture]
public class ChangePropertyPriceUseCaseTests
{
    private Mock<ILogger<ChangePropertyPriceUseCase>> _loggerMock;
    private Mock<IPropertyService> _propertyServiceMock;
    private Mock<IPropertyTraceService> _propertyTraceServiceMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private ChangePropertyPriceUseCase _useCase;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<ChangePropertyPriceUseCase>>();
        _propertyServiceMock = new Mock<IPropertyService>();
        _propertyTraceServiceMock = new Mock<IPropertyTraceService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _useCase = new ChangePropertyPriceUseCase(
            _loggerMock.Object,
            _propertyServiceMock.Object,
            _propertyTraceServiceMock.Object,
            _unitOfWorkMock.Object
        );
    }

    private static void VerifyLog(Mock<ILogger<ChangePropertyPriceUseCase>> loggerMock, LogLevel level,
        string containsMessage)
    {
        loggerMock.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(containsMessage)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );
    }

    [Test]
    public async Task Execute_ReturnsBadRequest_WhenPropertyDoesNotExist()
    {
        // Arrange
        var request = new ChangePropertyPriceRequest
        {
            PropertyId = Guid.NewGuid(),
            NewPrice = 100m
        };
        _propertyServiceMock
            .Setup(s => s.AnyAsync(It.IsAny<Expression<Func<Property, bool>>>()))
            .ReturnsAsync(Result.Success(false));

        // Act
        var result = await _useCase.Execute(request);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.HttpStatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        VerifyLog(_loggerMock, LogLevel.Error, "Not Found Property");
    }

    [Test]
    public async Task Execute_ReturnsFailure_WhenUpdateFails()
    {
        // Arrange
        var request = new ChangePropertyPriceRequest
        {
            PropertyId = Guid.NewGuid(),
            NewPrice = 200m
        };
        _propertyServiceMock
            .Setup(s => s.AnyAsync(It.IsAny<Expression<Func<Property, bool>>>()))
            .ReturnsAsync(Result.Success(true));
        _unitOfWorkMock
            .Setup(u => u.BeginTransactionAsync())
            .Returns(Task.CompletedTask);
        _propertyServiceMock
            .Setup(s => s.UpdateTransactAsync(
                It.Is<PropertyPriceUpdateModel>(m => m.Id == request.PropertyId && m.Price == request.NewPrice)))
            .ReturnsAsync(Result.Failure<Unit>([Error.Create("Error updating price")],
                HttpStatusCode.InternalServerError));

        // Act
        var result = await _useCase.Execute(request);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.HttpStatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        VerifyLog(_loggerMock, LogLevel.Error, "Error updating price for Property");
        _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Never);
    }

    [Test]
    public async Task Execute_ReturnsFailure_WhenTraceInsertFails_AndRollbackIsCalled()
    {
        // Arrange
        var request = new ChangePropertyPriceRequest
        {
            PropertyId = Guid.NewGuid(),
            NewPrice = 300m
        };
        _propertyServiceMock
            .Setup(s => s.AnyAsync(It.IsAny<Expression<Func<Property, bool>>>()))
            .ReturnsAsync(Result.Success(true));
        _unitOfWorkMock
            .Setup(u => u.BeginTransactionAsync())
            .Returns(Task.CompletedTask);
        _propertyServiceMock
            .Setup(s => s.UpdateTransactAsync(It.IsAny<PropertyPriceUpdateModel>()))
            .ReturnsAsync(Result.Success(Unit.Value));
        _propertyTraceServiceMock
            .Setup(t => t.InsertTransactAsync(It.IsAny<PropertyTraceModel>()))
            .ReturnsAsync(Result.Failure<Guid>([Error.Create("Error inserting trace")],
                HttpStatusCode.InternalServerError));

        // Act
        var result = await _useCase.Execute(request);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.HttpStatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        VerifyLog(_loggerMock, LogLevel.Error, "Error inserting trace for price change");
        _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Once);
    }

    [Test]
    public async Task Execute_ReturnsSuccess_WhenEverythingIsValid()
    {
        // Arrange
        var request = new ChangePropertyPriceRequest
        {
            PropertyId = Guid.NewGuid(),
            NewPrice = 500m
        };
        _propertyServiceMock
            .Setup(s => s.AnyAsync(It.IsAny<Expression<Func<Property, bool>>>()))
            .ReturnsAsync(Result.Success(true));
        _unitOfWorkMock
            .Setup(u => u.BeginTransactionAsync())
            .Returns(Task.CompletedTask);
        _propertyServiceMock
            .Setup(s => s.UpdateTransactAsync(It.IsAny<PropertyPriceUpdateModel>()))
            .ReturnsAsync(Result.Success(Unit.Value));
        _propertyTraceServiceMock
            .Setup(t => t.InsertTransactAsync(It.IsAny<PropertyTraceModel>()))
            .ReturnsAsync(Result.Success(Guid.NewGuid()));
        _unitOfWorkMock
            .Setup(u => u.CommitAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.Execute(request);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.HttpStatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Execute_RollsBackAndReturnsFailure_WhenExceptionOccurs()
    {
        // Arrange
        var request = new ChangePropertyPriceRequest
        {
            PropertyId = Guid.NewGuid(),
            NewPrice = 600m
        };
        _propertyServiceMock
            .Setup(s => s.AnyAsync(It.IsAny<Expression<Func<Property, bool>>>()))
            .ReturnsAsync(Result.Success(true));
        _unitOfWorkMock
            .Setup(u => u.BeginTransactionAsync())
            .Returns(Task.CompletedTask);
        _propertyServiceMock
            .Setup(s => s.UpdateTransactAsync(It.IsAny<PropertyPriceUpdateModel>()))
            .ThrowsAsync(new Exception("Unexpected DB error"));

        // Act
        var result = await _useCase.Execute(request);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.HttpStatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        VerifyLog(_loggerMock, LogLevel.Error, "Error in ChangePriceUseCase");
        _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Once);
    }
}