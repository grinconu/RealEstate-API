using System.Net;
using System.Runtime.ExceptionServices;
using RealEstate.Shared.Entities.Response;

namespace RealEstate.Shared.Utilities;

public static class ResultSuccessHttpStatusCode
{
    public static async Task<Result<T>> UseSuccessHttpStatusCode<T>(this Task<Result<T>> result, HttpStatusCode httpStatusCode)
    {
        try
        {
            var r = await result;
            return r.UseSuccessHttpStatusCode(httpStatusCode);
        }
        catch (Exception e)
        {
            ExceptionDispatchInfo.Capture(e).Throw();
            throw;
        }
    }

    public static Result<T> UseSuccessHttpStatusCode<T>(this Result<T> r, HttpStatusCode httpStatusCode)
    {
        try
        {
            return r.Success
                ? r.Value.Success(httpStatusCode)
                : Result.Failure<T>(r.Errors, r.HttpStatusCode);
        }
        catch (Exception e)
        {
            ExceptionDispatchInfo.Capture(e).Throw();
            throw;
        }
    }
}