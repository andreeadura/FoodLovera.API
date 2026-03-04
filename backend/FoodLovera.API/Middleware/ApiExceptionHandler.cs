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
            // 400
            ArgumentException ae => ((int)HttpStatusCode.BadRequest, "Validation error", ae.Message),

            // 401
            AuthenticationException aue => ((int)HttpStatusCode.Unauthorized, "Unauthorized", aue.Message),

            // 404
            NotFoundException nfe => ((int)HttpStatusCode.NotFound, "Not found", nfe.Message),

            // 409
            ConflictException ce => ((int)HttpStatusCode.Conflict, "Conflict", ce.Message),

            // Optional: dacă încă mai ai vechi InvalidOperationException folosite ca "conflict"
            // (până cureți tot codul). Dacă vrei strict, scoate linia asta.
            InvalidOperationException ioe => ((int)HttpStatusCode.Conflict, "Conflict", ioe.Message),

            // 500
            _ => ((int)HttpStatusCode.InternalServerError, "Server error", "An unexpected error occurred.")
        };
}