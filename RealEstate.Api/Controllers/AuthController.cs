using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Domain.DTOs.Auth;
using RealEstate.Domain.DTOs.Config;

namespace RealEstate.Api.Controllers;

[Authorize]
[ApiController]
public class AuthController(IOptions<AuthenticationConfig> authConfig) : BaseController
{
    AuthenticationConfig _authConfig = authConfig.Value;
    
    /// <summary>Generates a JWT token to access protected endpoints.</summary>
    /// <remarks>
    /// Validates a pre-shared key (<c>AccessKey</c>) and issues a short-lived JWT (role: <c>test</c>).
    /// </remarks>
    /// <param name="request">Payload containing the <c>AccessKey</c>.</param>
    /// <response code="200">JWT successfully created.</response>
    /// <response code="401">When the provided <c>AccessKey</c> is invalid.</response>
    [HttpPost("token")]
    [AllowAnonymous]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GenerateToken([FromBody] AuthRequest request)
    {
        if (request.AccessKey != _authConfig.TokenAuth)
        {
            return Unauthorized(new { message = "Invalid AccessKey" });
        }
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, "test")
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authConfig.JwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        var expirationTokenMins = double.Parse(_authConfig.JwtTokenExpirationInMin);
        var expireDate = DateTime.UtcNow.AddMinutes(expirationTokenMins);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: expireDate,
            signingCredentials: creds
        );

        var tokenResult = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new AuthResponse
        {
            Token = tokenResult,
            Expiration = expireDate
        });
    }
}