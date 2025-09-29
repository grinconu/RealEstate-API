using System.Linq.Expressions;
using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RealEstate.Domain.DTOs.Property;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces.Services;
using RealEstate.Application.Implements.UseCases;
using RealEstate.Shared.Entities.Response;

namespace RealEstate.Application.Test.Application.Implements.UseCases;

[TestFixture]
public class CreatePropertyUseCaseTests
{
    private Mock<ILogger<CreatePropertyUseCase>> _loggerMock;
    private Mock<IOwnerService> _ownerServiceMock;
    private Mock<IPropertyService> _propertyServiceMock;
    private CreatePropertyUseCase _useCase;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<CreatePropertyUseCase>>();
        _ownerServiceMock = new Mock<IOwnerService>();
        _propertyServiceMock = new Mock<IPropertyService>();

        _useCase = new CreatePropertyUseCase(
            _loggerMock.Object,
            _ownerServiceMock.Object,
            _propertyServiceMock.Object
        );
    }

    [Test]
    public async Task Execute_ReturnsBadRequest_WhenOwnerDoesNotExist()
    {
        // Arrange
        var request = new CreatePropertyRequest
        {
            OwnerId = Guid.NewGuid(),
            Name = "Test Property"
        };

        _ownerServiceMock
            .Setup(s => s.AnyAsync(It.IsAny<Expression<Func<Owner, bool>>>()))
            .ReturnsAsync(Result.Success(false));

        // Act
        var result = await _useCase.Execute(request);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.HttpStatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Not Found Owner")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.Once
        );
    }

    [Test]
    public async Task Execute_ReturnsFailure_WhenInsertFails()
    {
        // Arrange
        var request = new CreatePropertyRequest
        {
            OwnerId = Guid.NewGuid(),
            Name = "Test Property"
        };

        _ownerServiceMock
            .Setup(s => s.AnyAsync(It.IsAny<Expression<Func<Owner, bool>>>()))
            .ReturnsAsync(Result.Success(true));

        _propertyServiceMock
            .Setup(s => s.InsertAsync(It.IsAny<CreatePropertyRequest>()))
            .ReturnsAsync(Result.Success(Guid.Empty)); // simula error

        // Act
        var result = await _useCase.Execute(request);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.HttpStatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error to insert Property")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.Once
        );
    }

    [Test]
    public async Task Execute_ReturnsSuccess_WhenInsertIsSuccessful()
    {
        // Arrange
        var request = new CreatePropertyRequest
        {
            OwnerId = Guid.NewGuid(),
            Name = "Test Property"
        };

        _ownerServiceMock
            .Setup(s => s.AnyAsync(It.IsAny<Expression<Func<Owner, bool>>>()))
            .ReturnsAsync(Result.Success(true));

        _propertyServiceMock
            .Setup(s => s.InsertAsync(It.IsAny<CreatePropertyRequest>()))
            .ReturnsAsync(Result.Success(Guid.NewGuid()));

        // Act
        var result = await _useCase.Execute(request);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.HttpStatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}