using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SagradaFamilia.Application.Interfaces.Repositories;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Application.Interfaces.Services.External;
using SagradaFamilia.Application.Mappings;
using SagradaFamilia.Application.Services;
using SagradaFamilia.Application.Validators.Auth;
using SagradaFamilia.Domain.Interfaces.Repositories;
using SagradaFamilia.Infrastructure.Authentication;
using SagradaFamilia.Infrastructure.ExternalServices;
using SagradaFamilia.Infrastructure.Persistence.Contexts;
using SagradaFamilia.Infrastructure.Persistence.Repositories;
using System.Text;
using QuestPDF.Infrastructure;

namespace SagradaFamilia.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sql => sql.MigrationsAssembly("SagradaFamilia.Infrastructure")
                )
            );
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

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IRolRepository, RolRepository>();
            services.AddScoped<IAccionRepository, AccionRepository>();
            services.AddScoped<INinoRepository, NinoRepository>();
            services.AddScoped<IMedidaRepository, MedidaRepository>();
            services.AddScoped<IPrediccionRepository, PrediccionRepository>();
            services.AddScoped<IOmsRepository, OmsRepository>();
            services.AddScoped<IAlimentoRepository, AlimentoRepository>();
            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<INinoService, NinoService>();
            services.AddScoped<IMedidaService, MedidaService>();
            services.AddScoped<IPrediccionService, PrediccionService>();
            services.AddScoped<IAlimentoService, AlimentoService>();
            services.AddScoped<ITokenService, TokenService>();
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
            services.AddAutoMapper(
                typeof(AuthMappingProfile),
                typeof(MenuMappingProfile),
                typeof(NinoMappingProfile),
                typeof(MedidaMappingProfile),
                typeof(PrediccionMappingProfile),
                typeof(AlimentoMappingProfile)
            );
            return services;
        }

        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<LoginValidator>();
            return services;
        }

        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
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

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Ingresá el token JWT"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id   = "Bearer"
                        }
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
