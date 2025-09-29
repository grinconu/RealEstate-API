using System.Collections.Immutable;
using System.Text.Json;
using RealEstate.Shared.Entities;

namespace RealEstate.Api.Utilities;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new ResultModel<Unit>(ImmutableArray.Create(
                Shared.Entities.Error.Create(
                    "An unexpected error occurred. Please try again later.",
                    ((int)HttpStatusCode.InternalServerError).ToString(),
                    ex.ToString()
                )));

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}