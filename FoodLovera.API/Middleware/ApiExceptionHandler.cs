using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FoodLovera.Core.Exceptions;

namespace FoodLovera.API.Middleware;

public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        var (status, title, detail) = Map(exception);

        if (status >= 500)
            logger.LogError(exception, "Unhandled exception");
        else
            logger.LogWarning(exception, "Request failed: {Title}", title);

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        problem.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problem, ct);
        return true;
    }

    private static (int Status, string Title, string Detail) Map(Exception ex) =>
        ex switch
        {
            ArgumentException ae => ((int)HttpStatusCode.BadRequest, "Validation error", ae.Message),
            NotFoundException nfe => ((int)HttpStatusCode.NotFound, "Not found", nfe.Message),
            InvalidOperationException ioe => ((int)HttpStatusCode.Conflict, "Conflict", ioe.Message),
            _ => ((int)HttpStatusCode.InternalServerError, "Server error", "An unexpected error occurred.")
        };
}