namespace RealEstate.Domain.Utilities;

public static class FileExtension
{
    private static readonly HashSet<string> _validImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png"
    };

    /// <summary>
    /// Verify whether an extension corresponds to a valid image format.
    /// </summary>
    public static bool IsValidImageExtension(this string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
            return false;

        return _validImageExtensions.Contains(extension);
    }
}