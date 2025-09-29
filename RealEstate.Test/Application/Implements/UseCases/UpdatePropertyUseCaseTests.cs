using System.Linq.Expressions;
using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RealEstate.Domain.DTOs.Image;
using RealEstate.Domain.DTOs.Property;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces.Services;
using RealEstate.Domain.Interfaces.Utilities;
using RealEstate.Application.Implements.UseCases;
using RealEstate.Shared.Entities;
using Result = RealEstate.Shared.Entities.Response.Result;

namespace RealEstate.Application.Test.Application.Implements.UseCases;

[TestFixture]
public class UpdatePropertyUseCaseTests
{
    private Mock<ILogger<UpdatePropertyUseCase>> _loggerMock;
    private Mock<IPropertyService> _propertyServiceMock;
    private Mock<IPropertyImageService> _propertyImageServiceMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private UpdatePropertyUseCase _useCase;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<UpdatePropertyUseCase>>();
        _propertyServiceMock = new Mock<IPropertyService>();
        _propertyImageServiceMock = new Mock<IPropertyImageService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _useCase = new UpdatePropertyUseCase(
            _loggerMock.Object,
            _propertyServiceMock.Object,
            _propertyImageServiceMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Test]
    public async Task Execute_ReturnsBadRequest_WhenPropertyDoesNotExist()
    {
        // Arrange
        var request = new UpdatePropertyRequest { Id = Guid.NewGuid() };

        _propertyServiceMock
            .Setup(s => s.AnyAsync(It.IsAny<Expression<Func<Property, bool>>>()))
            .ReturnsAsync(Result.Success(false));

        // Act
        var result = await _useCase.Execute(request);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.HttpStatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Execute_ReturnsFailure_WhenUpdatePropertyFails()
    {
        // Arrange
        var request = new UpdatePropertyRequest { Id = Guid.NewGuid() };

        _propertyServiceMock
            .Setup(s => s.AnyAsync(It.IsAny<Expression<Func<Property, bool>>>()))
            .ReturnsAsync(Result.Success(true));

        _propertyServiceMock
            .Setup(s => s.UpdateTransactAsync(It.IsAny<PropertyUpdateModel>()))
            .ReturnsAsync(Result.Failure<Unit>(
                [Error.Create("Error updating property")],
                HttpStatusCode.InternalServerError
            ));

        // Act
        var result = await _useCase.Execute(request);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.HttpStatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Once);
    }

    [Test]
    public async Task Execute_ReturnsFailure_WhenUpdateImageFails()
    {
        // Arrange
        var request = new UpdatePropertyRequest
        {
            Id = Guid.NewGuid(),
            Images = new List<UpdatePropertyImageRequest>
            {
                new() { Id = Guid.NewGuid(), IsEnabled = false }
            }
        };

        _propertyServiceMock
            .Setup(s => s.AnyAsync(It.IsAny<Expression<Func<Property, bool>>>()))
            .ReturnsAsync(Result.Success(true));

        _propertyServiceMock
            .Setup(s => s.UpdateTransactAsync(It.IsAny<PropertyUpdateModel>()))
            .ReturnsAsync(Result.Success(Unit.Value));

        _propertyImageServiceMock
            .Setup(s => s.UpdateTransactAsync(It.IsAny<PropertyImageUpdateModel>()))
            .ReturnsAsync(Result.Failure<Unit>(
                [Error.Create("Error updating property images")],
                HttpStatusCode.InternalServerError
            ));

        // Act
        var result = await _useCase.Execute(request);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.HttpStatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Once);
    }

    [Test]
    public async Task Execute_ReturnsSuccess_WhenUpdateIsSuccessful()
    {
        // Arrange
        var request = new UpdatePropertyRequest
        {
            Id = Guid.NewGuid(),
            Images = new List<UpdatePropertyImageRequest>
            {
                new() { Id = Guid.NewGuid(), IsEnabled = true }
            }
        };

        _propertyServiceMock
            .Setup(s => s.AnyAsync(It.IsAny<Expression<Func<Property, bool>>>()))
            .ReturnsAsync(Result.Success(true));

        _propertyServiceMock
            .Setup(s => s.UpdateTransactAsync(It.IsAny<PropertyUpdateModel>()))
            .ReturnsAsync(Result.Success(Unit.Value));

        _propertyImageServiceMock
            .Setup(s => s.UpdateTransactAsync(It.IsAny<PropertyImageUpdateModel>()))
            .ReturnsAsync(Result.Success(Unit.Value));

        // Act
        var result = await _useCase.Execute(request);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.HttpStatusCode, Is.EqualTo(HttpStatusCode.OK));
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Test]
    public async Task Execute_ReturnsFailure_WhenExceptionIsThrown()
    {
        // Arrange
        var request = new UpdatePropertyRequest { Id = Guid.NewGuid() };

        _propertyServiceMock
            .Setup(s => s.AnyAsync(It.IsAny<Expression<Func<Property, bool>>>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _useCase.Execute(request);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.HttpStatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Once);
    }
}