using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstate.Domain.Interfaces.ExternalServices;
using RealEstate.Domain.Interfaces.Repositories;
using RealEstate.Domain.Interfaces.Utilities;
using RealEstate.Infrastructure.Context;
using RealEstate.Infrastructure.ExternalServices;
using RealEstate.Infrastructure.Repositories;
using RealEstate.Infrastructure.Utilities;

namespace RealEstate.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureDependencies(
        this IServiceCollection service,
        IConfiguration configuration)
    {
        service
            .AddDbContext<RealEstateDbContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("DBConnection")
                                 ?? throw new ArgumentException("Connection string must be defined"));
                opt.EnableSensitiveDataLogging(false);
            })
            .AddScoped<DbContext, RealEstateDbContext>()
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddExternalServices()
            .AddEntityRepositories();
    }

    private static IServiceCollection AddEntityRepositories(this IServiceCollection service)
    {
        return service
            .AddScoped<IOwnerRepository, OwnerRepository>()
            .AddScoped<IPropertyImageRepository, PropertyImageRepository>()
            .AddScoped<IPropertyRepository, PropertyRepository>()
            .AddScoped<IPropertyTraceRepository, PropertyTraceRepository>();
    }

    private static IServiceCollection AddExternalServices(this IServiceCollection service)
    {
        return service
            .AddScoped<IFileStorageService, FileStorageService>();
    }
}