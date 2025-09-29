using System.Linq.Expressions;
using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RealEstate.Domain.DTOs.Image;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces.ExternalServices;
using RealEstate.Domain.Interfaces.Services;
using RealEstate.Application.Implements.UseCases;
using RealEstate.Application.Test.Utilities;
using RealEstate.Shared.Entities;
using RealEstate.Shared.Entities.Response;

namespace RealEstate.Application.Test.Application.Implements.UseCases;

[TestFixture]
public class AddImageToPropertyUseCaseTests
{
    private Mock<ILogger<AddImageToPropertyUseCase>> _loggerMock;
    private Mock<IPropertyService> _propertyServiceMock;
    private Mock<IPropertyImageService> _propertyImageServiceMock;
    private Mock<IFileStorageService> _fileStorageServiceMock;
    private AddImageToPropertyUseCase _useCase;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<AddImageToPropertyUseCase>>();
        _propertyServiceMock = new Mock<IPropertyService>();
        _propertyImageServiceMock = new Mock<IPropertyImageService>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();

        _useCase = new AddImageToPropertyUseCase(
            _loggerMock.Object,
            _propertyServiceMock.Object,
            _propertyImageServiceMock.Object,
            _fileStorageServiceMock.Object
        );
    }

    private static void VerifyLog(Mock<ILogger<AddImageToPropertyUseCase>> loggerMock, LogLevel level, string containsMessage)
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
        var fileName = "test-image.png";
        var content = new byte[] { 0xFF, 0xD8, 0xFF };
        var formFile = TestFileHelpers.CreateTestFormFile(fileName, content);
        var request = new AddImageToPropertyRequest
        {
            PropertyId = Guid.NewGuid(),
            FileName = fileName,
            File = formFile
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
    public async Task Execute_ReturnsBadRequest_WhenFileHasInvalidExtension()
    {
        // Arrange
        var fileName = "text.txt";
        var content = new byte[] { 0xFF, 0xD8, 0xFF };
        var formFile = TestFileHelpers.CreateTestFormFile(fileName, content);
        var request = new AddImageToPropertyRequest
        {
            PropertyId = Guid.NewGuid(),
            FileName = fileName,
            File = formFile
        };
        _propertyServiceMock
            .Setup(s => s.AnyAsync(It.IsAny<Expression<Func<Property, bool>>>()))
            .ReturnsAsync(Result.Success(true));

        // Act
        var result = await _useCase.Execute(request);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.HttpStatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        VerifyLog(_loggerMock, LogLevel.Error, "Invalid file extension");
    }

    [Test]
    public async Task Execute_ReturnsFailure_WhenUploadImageFails()
    {
        // Arrange
        var fileName = "image.jpg";
        var content = new byte[] { 0xFF, 0xD8, 0xFF };
        var formFile = TestFileHelpers.CreateTestFormFile(fileName, content);
        var request = new AddImageToPropertyRequest
        {
            PropertyId = Guid.NewGuid(),
            FileName = fileName,
            File = formFile
        };
        _propertyServiceMock
            .Setup(s => s.AnyAsync(It.IsAny<Expression<Func<Property, bool>>>()))
            .ReturnsAsync(Result.Success(true));
        _fileStorageServiceMock
            .Setup(s => s.UploadImageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
            .ReturnsAsync(Result.Failure<string>([Error.Create("Upload failed")], HttpStatusCode.InternalServerError));

        // Act
        var result = await _useCase.Execute(request);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.HttpStatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        VerifyLog(_loggerMock, LogLevel.Error, "Error uploading image");
    }

    [Test]
    public async Task Execute_DeletesImageFromStorage_WhenInsertFails()
    {
        // Arrange
        var fileName = "image.png";
        var content = new byte[] { 0xFF, 0xD8, 0xFF };
        var formFile = TestFileHelpers.CreateTestFormFile(fileName, content);
        var request = new AddImageToPropertyRequest
        {
            PropertyId = Guid.NewGuid(),
            FileName = fileName,
            File = formFile
        };
        _propertyServiceMock
            .Setup(s => s.AnyAsync(It.IsAny<Expression<Func<Property, bool>>>()))
            .ReturnsAsync(Result.Success(true));
        _fileStorageServiceMock
            .Setup(s => s.UploadImageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
            .ReturnsAsync(Result.Success("url-to-image"));
        _propertyImageServiceMock
            .Setup(s => s.InsertAsync(It.IsAny<PropertyImage>()))
            .ReturnsAsync(Result.Success(Guid.Empty));

        // Act
        var result = await _useCase.Execute(request);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.HttpStatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        VerifyLog(_loggerMock, LogLevel.Error, "Error saving image record");

        _fileStorageServiceMock.Verify(
            s => s.DeleteImageAsync(It.IsAny<string>(), It.IsAny<string>()),
            Times.Once
        );
    }

    [Test]
    public async Task Execute_ReturnsSuccess_WhenImageIsInsertedSuccessfully()
    {
        // Arrange
        var fileName = "image.jpg";
        var content = new byte[] { 0xFF, 0xD8, 0xFF };
        var formFile = TestFileHelpers.CreateTestFormFile(fileName, content);
        var request = new AddImageToPropertyRequest
        {
            PropertyId = Guid.NewGuid(),
            FileName = fileName,
            File = formFile
        };
        _propertyServiceMock
            .Setup(s => s.AnyAsync(It.IsAny<Expression<Func<Property, bool>>>()))
            .ReturnsAsync(Result.Success(true));
        _fileStorageServiceMock
            .Setup(s => s.UploadImageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
            .ReturnsAsync(Result.Success("url-to-image"));
        _propertyImageServiceMock
            .Setup(s => s.InsertAsync(It.IsAny<PropertyImage>()))
            .ReturnsAsync(Result.Success(Guid.NewGuid()));

        // Act
        var result = await _useCase.Execute(request);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.HttpStatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Execute_WhenUnexpectedExceptionThrown_ShouldDeleteUploadedImage()
    {
        // Arrange
        var fileName = "image.jpg";
        var content = new byte[] { 0xFF, 0xD8, 0xFF };
        var formFile = TestFileHelpers.CreateTestFormFile(fileName, content);
        var request = new AddImageToPropertyRequest
        {
            PropertyId = Guid.NewGuid(),
            FileName = fileName,
            File = formFile
        };
        _propertyServiceMock
            .Setup(s => s.AnyAsync(It.IsAny<Expression<Func<Property, bool>>>()))
            .ReturnsAsync(Result.Success(true));
        _fileStorageServiceMock
            .Setup(s => s.UploadImageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
            .ReturnsAsync(Result.Success("url-to-image"));
        _propertyImageServiceMock
            .Setup(s => s.InsertAsync(It.IsAny<PropertyImage>()))
            .ThrowsAsync(new InvalidOperationException("DB failure"));

        // Act
        var result = await _useCase.Execute(request);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.HttpStatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        VerifyLog(_loggerMock, LogLevel.Error, "Error in AddImageToPropertyUseCase for Property");
        _fileStorageServiceMock.Verify(
            s => s.DeleteImageAsync(
                request.PropertyId.ToString(),
                It.Is<string>(fn => fn.EndsWith(".jpg"))),
            Times.Once
        );
    }
}