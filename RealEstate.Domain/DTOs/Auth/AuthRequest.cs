using System.ComponentModel.DataAnnotations;

namespace RealEstate.Domain.DTOs.Auth;

public class AuthRequest
{
    [Required]
    public string AccessKey { get; set; } = null!;
}