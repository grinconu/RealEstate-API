using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RealEstate.Domain.Utilities.Validations;

public class MaxFileSizeAttribute(long maxBytes) : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var file = value as IFormFile;
        if (file != null)
        {
            if (file.Length > maxBytes)
            {
                return new ValidationResult($"Maximum allowed file size is { maxBytes } bytes.");
            }
        }
        return ValidationResult.Success;
    }
}