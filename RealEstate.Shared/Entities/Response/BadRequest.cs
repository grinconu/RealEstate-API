using System.Collections.Immutable;
using System.Net;

namespace RealEstate.Shared.Entities.Response;

public static partial class Result
{
    public static Result<T> BadRequest<T>(string error, string? code = null) 
        => new(ImmutableArray.Create(Error.Create(error, $"{(int)HttpStatusCode.BadRequest}{code}")), HttpStatusCode.BadRequest);

    public static Result<T> BadRequest<T>(IEnumerable<string> errors, string? code = null) 
        => new(errors.Select(e => Error.Create(e, $"{(int)HttpStatusCode.BadRequest}{code}")).ToImmutableArray(), 
            HttpStatusCode.BadRequest);

    public static Result<T> BadRequest<T, TU>(Result<TU> r)
        => new(ImmutableArray.Create(Error.Create(r.Errors.First().Message)), r.HttpStatusCode);
}