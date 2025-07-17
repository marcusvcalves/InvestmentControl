using InvestmentControl.Domain.Models;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace InvestmentControl.ApplicationCore.Middlewares;

public class ExceptionHandlerMiddleware : IExceptionHandler
{
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    async ValueTask<bool> IExceptionHandler.TryHandleAsync(HttpContext httpContext,
        Exception ex, CancellationToken cancellationToken)
    {
        _logger.LogError(ex, "An unhandled exception occurred while processing the request.");

        var apiErrorResponse = new ApiErrorResponse(HttpStatusCode.InternalServerError,
            "Internal server error.");

        httpContext.Response.StatusCode = (int)apiErrorResponse.HttpStatusCode;

        await httpContext.Response.WriteAsJsonAsync(apiErrorResponse, cancellationToken);

        return true;
    }
}
