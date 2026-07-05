using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Repositories;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Application.Interfaces.Services.External;
using SagradaFamilia.Application.Mappings;
using SagradaFamilia.Application.Services;
using SagradaFamilia.Application.Settings;
using SagradaFamilia.Application.Validators.Auth;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Interfaces.Repositories;
using SagradaFamilia.Infrastructure.Authentication;
using SagradaFamilia.Infrastructure.Email;
using SagradaFamilia.Infrastructure.ExternalServices;
using SagradaFamilia.Infrastructure.Persistence.Contexts;
using SagradaFamilia.Infrastructure.Persistence.Repositories;
using System.Text;

namespace SagradaFamilia.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<PostgresAppDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("PostgresConnection"),
                    npgsql => npgsql
                        .MigrationsAssembly("SagradaFamilia.Infrastructure")
                        .MigrationsHistoryTable("__EFMigrationsHistory", "public")));

            services.AddScoped<AppDbContext>(sp => sp.GetRequiredService<PostgresAppDbContext>());

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var secretKey = configuration["JwtSettings:SecretKey"]!;
            var issuer = configuration["JwtSettings:Issuer"]!;
            var audience = configuration["JwtSettings:Audience"]!;

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                                                   Encoding.UTF8.GetBytes(secretKey)),
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = true,
                        ValidAudience = audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var token = context.Request.Cookies["access_token"];
                            if (!string.IsNullOrEmpty(token))
                            {
                                context.Token = token;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization();
            return services;
        }

        public static IServiceCollection AddEmailService(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<BrevoSettings>(configuration.GetSection("BrevoSettings"));
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
            services.AddHttpClient("Brevo", client =>
            {
                client.BaseAddress = new Uri("https://api.brevo.com/v3/");
                client.DefaultRequestHeaders.Add("api-key", configuration["BrevoSettings:ApiKey"]);
            });
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IPadreRepository, PadreRepository>();
            services.AddScoped<IMedicoRepository, MedicoRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IRolRepository, RolRepository>();
            services.AddScoped<IAccionRepository, AccionRepository>();
            services.AddScoped<INinoRepository, NinoRepository>();
            services.AddScoped<IMedidaRepository, MedidaRepository>();
            services.AddScoped<IPrediccionRepository, PrediccionRepository>();
            services.AddScoped<IPrescripcionRepository, PrescripcionRepository>();
            services.AddScoped<IOmsRepository, OmsRepository>();
            services.AddScoped<IAlimentoRepository, AlimentoRepository>();
            services.AddScoped<ICitaRepository, CitaRepository>();
            services.AddScoped<IConsultaRepository, ConsultaRepository>();
            services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();
            services.AddScoped<ILogSistemaRepository, LogSistemaRepository>();
            services.AddScoped<IParametroRepository, ParametroRepository>();
            services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
            services.AddScoped<IPlantillaCorreoRepository, PlantillaCorreoRepository>();
            services.AddScoped<IEventoCorreoRepository, EventoCorreoRepository>();

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPadreService, PadreService>();
            services.AddScoped<IMedicoService, MedicoService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<INinoService, NinoService>();
            services.AddScoped<IMedidaService, MedidaService>();
            services.AddScoped<IPrediccionService, PrediccionService>();
            services.AddScoped<IPrescripcionService, PrescripcionService>();
            services.AddScoped<IAlimentoService, AlimentoService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICitaService, CitaService>();
            services.AddScoped<IConsultaService, ConsultaService>();

            services.AddScoped<IAuditoriaService, AuditoriaService>();
            services.AddScoped<ILogSistemaService, LogSistemaService>();
            services.AddScoped<IParametroService, ParametroService>();
            services.AddScoped<IPlantillaCorreoService, PlantillaCorreoService>();
            services.AddScoped<IEventoCorreoService, EventoCorreoService>();
            services.AddScoped<IDashboardAdminService, DashboardAdminService>();

            return services;
        }

        public static IServiceCollection AddProphetClient(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHttpClient<IProphetApiClient, ProphetApiClient>(client =>
            {
                client.BaseAddress = new Uri(
                    configuration["ProphetApi:BaseUrl"] ?? "http://localhost:8000");
                client.Timeout = TimeSpan.FromSeconds(30);
            });
            return services;
        }

        public static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AuthMappingProfile).Assembly);
            return services;
        }

        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssembly(typeof(AuthService).Assembly);

            // Unifica el formato de errores de validación con ApiResponse.Fail
            // para que el frontend siempre reciba { success: false, message: "..." }
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errores = context.ModelState
                        .Where(ms => ms.Value?.Errors.Count > 0)
                        .SelectMany(ms => ms.Value!.Errors)
                        .Select(e => e.ErrorMessage);

                    return new BadRequestObjectResult(ApiResponse.Fail(string.Join(" | ", errores)));
                };
            });

            return services;
        }

        public static IServiceCollection AddHttpContext(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            return services;
        }

        public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            var frontendUrl = configuration["AppSettings:FrontendUrl"] ?? string.Empty;
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    var origins = new List<string> { "http://localhost:4200" };
                    if (!string.IsNullOrWhiteSpace(frontendUrl) && !origins.Contains(frontendUrl))
                        origins.Add(frontendUrl);

                    policy.WithOrigins([.. origins])
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Sagrada Familia API",
                    Version = "v1",
                    Description = "Sistema de monitoreo de crecimiento infantil"
                });

                options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Ingresá el token JWT directamente o se tomará de la cookie 'access_token'"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            return services;
        }

        public static IServiceCollection AddPdfReporting(this IServiceCollection services)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            return services;
        }
    }
}
