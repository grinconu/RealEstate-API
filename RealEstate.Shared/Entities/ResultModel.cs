using System.Collections.Immutable;

namespace RealEstate.Shared.Entities;

public record ResultModel<T>
{
    public T Value { get; private set; }
    public ImmutableArray<Error> Errors { get; set; }
    public bool Success => Errors.Length == 0;

    public ResultModel()
    {
        Value = default;
        Errors = ImmutableArray<Error>.Empty;
    }

    public ResultModel(T value)
    {
        Value = value;
        Errors = ImmutableArray<Error>.Empty;
    }

    public ResultModel(ImmutableArray<Error> errors)
    {
        Value = default;
        Errors = errors;
    }
}