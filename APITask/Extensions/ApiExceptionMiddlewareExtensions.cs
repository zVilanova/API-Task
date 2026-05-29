using APITask.Models.Errors;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace APITask.Extensions;

public static class ApiExceptionMiddlewareExtensions
{
    // "this" significa que o método é uma extensão da interface IApplicationBuilder
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        // Configura o middleware recebendo um delegate para tratamento de exceções globais
        app.UseExceptionHandler(appError =>
        {
            // Define a resposta enviada quando uma exceção não tratada ocorrer
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                // Recupera os detalhes da exceção capturada pelo middleware.
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                if (contextFeature != null)
                {
                    var logger = context.RequestServices
                        .GetRequiredService<ILoggerFactory>() // Pegue a fabrica de loggers do container DI
                        .CreateLogger("GlobalExceptionHandler"); // Crie um logger chamado GlobalExceptionHandler

                    logger.LogError(contextFeature.Error, 
                                    "Unhandled exception. TraceId: {TraceId}", 
                                    context.TraceIdentifier);

                    await context.Response.WriteAsync(new ErrorDetails()
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = "Ocorreu um erro interno no servidor.",
                        TraceId = context.TraceIdentifier
                    }.ToString());
                }
            });
        });
    }
}
