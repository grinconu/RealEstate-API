using RealEstate.Domain.DTOs.Config;

namespace RealEstate.Api.Utilities;

public static class ConfigService
{
    public static void ConfigServicesDependencyInjection(this WebApplicationBuilder builder)
    {
        builder.Services
            .Configure<AuthenticationConfig>(builder.Configuration.GetSection("AuthenticationConfig"));
    }
}