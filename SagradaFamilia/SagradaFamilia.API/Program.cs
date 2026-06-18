using SagradaFamilia.API.Extensions;
using SagradaFamilia.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ── Servicios ───────────────────────────────────────────────────────────
builder.Services
    .AddDatabase(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddEmailService(builder.Configuration)
    .AddRepositories()
    .AddApplicationServices()
    .AddInfrastructureServices()
    .AddPdfReporting()
    .AddProphetClient(builder.Configuration)
    .AddAutoMapperProfiles()
    .AddValidators()
    .AddHttpContext()
    .AddCorsPolicy(builder.Configuration)
    .AddSwagger()
    .AddControllers();

builder.Services.AddEndpointsApiExplorer();

// ── Aplicación ──────────────────────────────────────────────────────────
var app = builder.Build();

// Migraciones y seeds automáticos al arrancar
await app.ApplyMigrationsAndSeedsAsync();

if (app.Environment.IsDevelopment())
    app.UseSwaggerDocs();

app.UseCustomMiddlewares();
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok());

app.Run();