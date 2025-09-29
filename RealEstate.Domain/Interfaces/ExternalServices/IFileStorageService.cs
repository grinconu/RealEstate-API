using RealEstate.Shared.Entities.Response;

namespace RealEstate.Domain.Interfaces.ExternalServices;

public interface IFileStorageService
{
    Task<Result<string>> UploadImageAsync(string folder,string fileName, Stream fileStream);
    
    Task<Result<Boolean>> DeleteImageAsync(string folder,string fileName);
}