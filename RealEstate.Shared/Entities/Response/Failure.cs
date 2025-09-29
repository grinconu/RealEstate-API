using System.Collections.Immutable;
using System.Net;

namespace RealEstate.Shared.Entities.Response;

public static partial class Result
{
    public static Result<T> Failure<T>(ImmutableArray<Error> errors, HttpStatusCode httpStatusCode) 
        => new (errors, httpStatusCode);

    public static Result<Unit> Failure(ImmutableArray<Error> errors, HttpStatusCode httpStatusCode)
        => new (errors, httpStatusCode);
}