using RealEstate.Domain.Interfaces.ExternalServices;
using RealEstate.Shared.Entities.Response;

namespace RealEstate.Infrastructure.ExternalServices;

public class FileStorageService : IFileStorageService
{
    // This is a fake implementation for testing purposes only.
    // The real implementation is not provided because we don't want
    // to depend on external storage configuration at this stage.
    public Task<Result<string>> UploadImageAsync(string folder, string fileName, Stream fileStream)
    {
        // Return a fake URL simulating that the file was uploaded successfully
        var fakeUrl = $"https://fakestorage.com/{folder}/{fileName}";
        return Task.FromResult(Result.Success(fakeUrl));
    }

    public Task<Result<bool>> DeleteImageAsync(string folder, string fileName)
    {
        // Simulate successful deletion
        return Task.FromResult(Result.Success(true));
    }
}