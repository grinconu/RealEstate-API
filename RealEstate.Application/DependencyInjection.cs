using System.Reflection;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstate.Domain.Interfaces.Services;
using RealEstate.Application.Implements.Services;
using RealEstate.Application.Implements.UseCases;

namespace RealEstate.Application;

public static class DependencyInjection
{
    public static void AddApplicationDependencies(this IServiceCollection service, IConfiguration configuration)
    {
        service
            .AddMappers()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
            .AddEntityServicesDi()
            .AddUseCasesDi();
    }

    private static IServiceCollection AddEntityServicesDi(this IServiceCollection service)
    {
        return service
            .AddScoped<IOwnerService, OwnerService>()
            .AddScoped<IPropertyImageService, PropertyImageService>()
            .AddScoped<IPropertyService, PropertyService>()
            .AddScoped<IPropertyTraceService, PropertyTraceService>();
    }

    private static void AddUseCasesDi(this IServiceCollection service)
    {
        service
            .AddScoped<CreatePropertyUseCase>()
            .AddScoped<AddImageToPropertyUseCase>()
            .AddScoped<ChangePropertyPriceUseCase>()
            .AddScoped<UpdatePropertyUseCase>()
            .AddScoped<ListPropertyWithFiltersUseCase>();
    }
    
    private static IServiceCollection AddMappers(this IServiceCollection service)
    {
        var config = new TypeAdapterConfig(); 
        config.Scan(Assembly.GetExecutingAssembly());
        
        service.AddSingleton(config);
        service.AddScoped<IMapper, ServiceMapper>();
        
        return service;
    }
}