using System.Collections.Immutable;
using System.Net;

namespace RealEstate.Shared.Entities.Response;

public static partial class Result
{
    public static Result<T> InternalError<T>(string error)
        => new(ImmutableArray.Create(Error.Create(error)), HttpStatusCode.InternalServerError);

    public static Result<Unit> InternalError(string error)
        => new(ImmutableArray.Create(Error.Create(error)), HttpStatusCode.InternalServerError);
}