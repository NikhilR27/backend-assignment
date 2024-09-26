using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using OT.Assignment.Domain.Models;

namespace OT.Assessment.App.Middleware;

public static class ExceptionMiddlewareExtensions
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILogger logger)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                IExceptionHandlerFeature contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    logger.LogError("Something went wrong: {ContextFeatureError}", contextFeature.Error);
                    await context.Response.WriteAsync(new ErrorDetails()
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = "Internal Server Error in Controller."
                    }.ToString());
                }
            });
        });
    }
}