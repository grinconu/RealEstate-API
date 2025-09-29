using Mapster;
using RealEstate.Domain.DTOs.PropertyTrace;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Mappers;

public class PropertyTraceMap : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<PropertyTraceModel, PropertyTrace>();
    }
}