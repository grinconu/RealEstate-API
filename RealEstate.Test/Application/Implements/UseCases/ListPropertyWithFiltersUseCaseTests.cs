using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RealEstate.Application.Implements.UseCases;
using RealEstate.Domain.DTOs.Property;
using RealEstate.Domain.Interfaces.Services;
using RealEstate.Shared.Entities;
using Result = RealEstate.Shared.Entities.Response.Result;

namespace RealEstate.Application.Test.Application.Implements.UseCases;

[TestFixture]
public class ListPropertyWithFiltersUseCaseTests
{
    private Mock<ILogger<ListPropertyWithFiltersUseCase>> _loggerMock;
    private Mock<IPropertyService> _propertyServiceMock;
    private ListPropertyWithFiltersUseCase _useCase;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<ListPropertyWithFiltersUseCase>>();
        _propertyServiceMock = new Mock<IPropertyService>();

        _useCase = new ListPropertyWithFiltersUseCase(
            _loggerMock.Object,
            _propertyServiceMock.Object
        );
    }

    [Test]
    public async Task Execute_ReturnsSuccess_WhenServiceReturnsData()
    {
        // Arrange
        var request = new ListPropertyFiltersRequest { Name = "Test" };
        var expectedPagedResult = new PagedResult<PropertyResponse>
        {
            Items = new List<PropertyResponse>
            {
                new() { Id = Guid.NewGuid(), Name = "Test Property", Address = "Somewhere" }
            },
            TotalCount = 1,
            Page = 1,
            PageSize = 10
        };
        _propertyServiceMock
            .Setup(s => s.GetPagedAsync(request))
            .ReturnsAsync(Result.Success(expectedPagedResult));

        // Act
        var result = await _useCase.Execute(request);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Value.TotalCount, Is.EqualTo(1));
        Assert.That(result.Value.Items, Has.Exactly(1).Items);
    }

    [Test]
    public async Task Execute_ReturnsFailure_WhenServiceFails()
    {
        // Arrange
        var request = new ListPropertyFiltersRequest { Name = "Test" };
        _propertyServiceMock
            .Setup(s => s.GetPagedAsync(request))
            .ReturnsAsync(Result.Failure<PagedResult<PropertyResponse>>(
                [Error.Create("Error fetching properties")],
                HttpStatusCode.InternalServerError
            ));

        // Act
        var result = await _useCase.Execute(request);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.HttpStatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Error fetching properties")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );
    }

    [Test]
    public async Task Execute_ReturnsFailure_WhenExceptionThrown()
    {
        // Arrange
        var request = new ListPropertyFiltersRequest { Name = "Test" };
        _propertyServiceMock
            .Setup(s => s.GetPagedAsync(request))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _useCase.Execute(request);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.HttpStatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Error in ListPropertyWithFiltersUseCase")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );
    }
}