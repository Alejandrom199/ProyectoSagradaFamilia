using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace SagradaFamilia.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error capturado en middleware en [{Method}] {Path}: {Message}",
                    context.Request.Method, context.Request.Path, ex.Message);

                await ManejarExcepcionAsync(context, ex);
            }
        }

        private async Task ManejarExcepcionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, mensaje) = ex switch
            {
                NotFoundException => (HttpStatusCode.NotFound, ex.Message),
                BusinessException => (HttpStatusCode.BadRequest, ex.Message),
                UnauthorizedException => (HttpStatusCode.Unauthorized, ex.Message),

                ValidationException vex => (HttpStatusCode.BadRequest,
                    string.Join(" | ", vex.Errors.SelectMany(kvp => kvp.Value))),

                _ => (HttpStatusCode.InternalServerError,
                      _env.IsDevelopment()
                          ? $"[DEV-ERROR]: {ex.Message} -> {ex.StackTrace}"
                          : "Ocurrió un error interno no controlado.")
            };

            context.Response.StatusCode = (int)statusCode;

            var response = ApiResponse.Fail(mensaje);

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
        }
    }
}