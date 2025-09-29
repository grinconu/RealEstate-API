using System.Net;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Shared.Entities;
using RealEstate.Shared.Entities.Response;

namespace RealEstate.Shared.Utilities;

public static class ActionResultExtensions
{
    public static async Task<IActionResult> ToActionResult<T>(this Task<Result<T>> result)
    {
        var r = (await result).ToActionResult();
        return r;
    }

    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        return result.ToResultModel().ToHttpStatusCode(result.HttpStatusCode);
    }
    private static ResultModel<T> ToResultModel<T>(this Result<T> result)
    {
        return result.Success
            ? new ResultModel<T>(result.Value)
            : new ResultModel<T>(result.Errors);
    }

    private static IActionResult ToHttpStatusCode<T>(this T resultModel, HttpStatusCode statusCode)
    {
        return new ResultWithStatusCode<T>(resultModel, statusCode);
    }

    private class ResultWithStatusCode<T> : ObjectResult
    {
        public ResultWithStatusCode(T content, HttpStatusCode httpStatusCode) : base(content)
        {
            StatusCode = (int)httpStatusCode;
        }
    }
}