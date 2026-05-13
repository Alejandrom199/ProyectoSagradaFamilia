using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace SagradaFamilia.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error capturado en middleware: {Message}", ex.Message);
                await ManejarExcepcionAsync(context, ex);
            }
        }

        private static async Task ManejarExcepcionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, mensaje) = ex switch
            {
                NotFoundException => (HttpStatusCode.NotFound, ex.Message),
                BusinessException => (HttpStatusCode.BadRequest, ex.Message),
                UnauthorizedException => (HttpStatusCode.Unauthorized, ex.Message),

                ValidationException vex => (HttpStatusCode.BadRequest,
                    string.Join(" | ", vex.Errors.SelectMany(kvp => kvp.Value))),

                _ => (HttpStatusCode.InternalServerError, "Ocurrió un error interno no controlado.")
            };

            context.Response.StatusCode = (int)statusCode;

            var response = ApiResponse.Fail(mensaje);
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}