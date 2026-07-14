namespace SagradaFamilia.Application.Services;

using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.Interfaces.Services;
using System.Reflection;

// Servicio liviano: solo expone el número de versión de la app, sin datos
// sensibles ni consultas a base de datos (a diferencia de IDashboardAdminService,
// que arma el dashboard completo de administración).
public class VersionService : IVersionService
{
    public Task<VersionDto> ObtenerAsync()
    {
        var version = Assembly.GetEntryAssembly()?
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion ?? "1.0.0";

        return Task.FromResult(new VersionDto { Version = version });
    }
}
