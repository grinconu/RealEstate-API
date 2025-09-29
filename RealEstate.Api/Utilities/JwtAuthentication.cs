using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Domain.DTOs.Config;

namespace RealEstate.Api.Utilities;

public static class JwtAuthentication
{
    public static void GetJwtAuthentication(this WebApplicationBuilder builder)
    {
        var authConfig = builder.Configuration
                             .GetSection("AuthenticationConfig")
                             .Get<AuthenticationConfig>() 
                         ?? throw new ArgumentException("The authentication must be configured");
        
        var key = Encoding.ASCII.GetBytes(authConfig.JwtSecret);
        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = true;
            x.SaveToken = true;
            x.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = AuthenticationFailed
            };
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
        });

        builder.Services.AddAuthorization();
    }

    private static Task AuthenticationFailed(AuthenticationFailedContext arg)
    {
        var error = $"AuthenticationFailed: {arg.Exception.Message}";
        arg.Response.ContentLength = error.Length;
        arg.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        arg.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(error), 0, error.Length);
        return Task.FromResult(0);
    }
}