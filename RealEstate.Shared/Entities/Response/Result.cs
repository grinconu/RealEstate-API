using System.Collections.Immutable;
using System.Net;

namespace RealEstate.Shared.Entities.Response;

public readonly struct Result<T>
{
    public readonly T Value;
    public readonly HttpStatusCode HttpStatusCode;
    public readonly ImmutableArray<Error> Errors;
    public bool Success => Errors.Length == 0;

    public static implicit operator Result<T>(T value) => new (value, HttpStatusCode.Accepted);

    public static implicit operator Result<T>(ImmutableArray<Error> errors) =>
        new Result<T>(errors, HttpStatusCode.BadRequest);

    public Result(T value, HttpStatusCode statusCode)
    {
        Value = value;
        HttpStatusCode = statusCode;
        Errors = ImmutableArray<Error>.Empty;
    }

    public Result(ImmutableArray<Error> errors, HttpStatusCode statusCode)
    {
        if (errors.Length == 0)
            throw new InvalidOperationException("should be almost an error");

        HttpStatusCode = statusCode;
        Value = default(T)!;
        Errors = errors;
    }
}

public static partial class Result
{
    public static readonly Unit Unit = Unit.Value;

    public static Result<T> Success<T>(this T value, HttpStatusCode httpStatusCode)
        => new(value, httpStatusCode);
    public static Result<T> Success<T>(this T value)
        => new(value, HttpStatusCode.Accepted);

    public static Result<Unit> Success()
        => new (Unit, HttpStatusCode.OK);
}