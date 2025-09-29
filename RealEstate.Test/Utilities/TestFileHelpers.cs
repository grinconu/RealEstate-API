using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace RealEstate.Application.Test.Utilities;

public static class TestFileHelpers
{
    public static IFormFile CreateTestFormFile(string fileName, byte[] content, string formName = "file")
    {
        var stream = new MemoryStream(content);
        stream.Position = 0;

        var formFile = new FormFile(
            baseStream: stream,
            baseStreamOffset: 0,
            length: content.Length,
            name: formName,
            fileName: fileName
        )
        {
            Headers = new HeaderDictionary(),
            ContentType = GetContentType(fileName)
        };

        return formFile;
    }

    private static string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }
}