using Bifrost.Core.Exception;
using Microsoft.AspNetCore.Diagnostics;

namespace Bifrost.Config;

public static class HandlerExceptionConfig
{
    public static void UseHandlerException(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
                var exception = exceptionFeature?.Error;

                var statusCode = exception switch
                {
                    CoreException coreEx => coreEx.StatusCode,
                    ArgumentException => StatusCodes.Status400BadRequest,
                    KeyNotFoundException => StatusCodes.Status404NotFound,
                    _ => StatusCodes.Status500InternalServerError
                };

                var logger = context.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("GlobalExceptionHandler");

                if (statusCode >= 500)
                    logger.LogError(exception, "Unhandled exception on {Method} {Path}",
                        context.Request.Method, context.Request.Path);
                else
                    logger.LogWarning(exception, "Client error {StatusCode} on {Method} {Path}",
                        statusCode, context.Request.Method, context.Request.Path);

                var message = statusCode < 500 ? exception?.Message : "Internal Server Error";

                await Results.Problem(
                    title: message,
                    statusCode: statusCode
                ).ExecuteAsync(context);
            });
        });
    }
}