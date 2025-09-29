using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using RealEstate.Domain.Utilities.Validations;

namespace RealEstate.Domain.DTOs.Image;

public class AddImageToPropertyRequest
{
    [JsonIgnore]
    public Guid PropertyId { get; set; } = Guid.Empty;
    
    [JsonIgnore]
    public string FileName { get; set; } = "";
    
    [Required]
    [MaxFileSize(5*1024*1024)] // 5 MB
    [AllowedExtensions([".jpg", ".jpeg", ".png"])]
    public IFormFile File { get; set; } = default!;
}