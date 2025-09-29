using System.Runtime.CompilerServices;
using Mapster;
using RealEstate.Domain.DTOs.Property;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Mappers;

public class PropertyMap : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreatePropertyRequest, Property>();
    }
}