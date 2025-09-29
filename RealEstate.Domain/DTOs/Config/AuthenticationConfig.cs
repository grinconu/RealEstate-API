namespace RealEstate.Domain.DTOs.Config;

public class AuthenticationConfig
{
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string HashSecret { get; set; } = null!;
    public string JwtSecret { get; set; } = null!;
    public string JwtTokenExpirationInMin { get; set; }
    public string TokenAuth { get; set; } = null!;
}