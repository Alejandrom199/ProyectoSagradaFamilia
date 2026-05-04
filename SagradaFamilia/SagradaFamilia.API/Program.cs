using SagradaFamilia.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ── Servicios ───────────────────────────────────────────────────────────
builder.Services
    .AddDatabase(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddRepositories()
    .AddApplicationServices()
    .AddPdfReporting()
    .AddProphetClient(builder.Configuration)
    .AddAutoMapperProfiles()
    .AddValidators()
    .AddCorsPolicy()
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
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();