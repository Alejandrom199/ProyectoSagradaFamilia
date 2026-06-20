namespace SagradaFamilia.Application.Services;

using AutoMapper;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Domain.Enums;
using SagradaFamilia.Domain.Interfaces.Repositories;
using System.Globalization;

public class DashboardAdminService : IDashboardAdminService
{
    private readonly IAuditoriaRepository   _auditoriaRepository;
    private readonly ILogSistemaRepository  _logRepository;
    private readonly IUsuarioRepository     _usuarioRepository;
    private readonly IMapper                _mapper;

    public DashboardAdminService(
        IAuditoriaRepository   auditoriaRepository,
        ILogSistemaRepository  logRepository,
        IUsuarioRepository     usuarioRepository,
        IMapper                mapper)
    {
        _auditoriaRepository = auditoriaRepository;
        _logRepository       = logRepository;
        _usuarioRepository   = usuarioRepository;
        _mapper              = mapper;
    }

    public async Task<SistemaDashboardDto> ObtenerAsync()
    {
        const int DiasActividad  = 7;
        const int MaxRecientes   = 8;
        const int MaxAlertasPool = 50;

        // EF Core no es thread-safe: el DbContext es Scoped (una instancia por request).
        // Las queries deben ejecutarse secuencialmente para evitar InvalidOperationException.
        var usuarios   = (await _usuarioRepository.ObtenerTodosAsync()).ToList();
        var accionesHoy = await _auditoriaRepository.ObtenerConteoHoyAsync();
        var fechas     = (await _auditoriaRepository.ObtenerFechasUltimosDiasAsync(DiasActividad)).ToList();
        var recientes  = await _auditoriaRepository.ObtenerRecientesAsync(MaxRecientes);
        var errores    = (await _logRepository.ObtenerErroresRecientesAsync(MaxAlertasPool)).ToList();

        var hace7dias = DateTime.UtcNow.AddDays(-DiasActividad);

        // Actividad por día (últimos 7 días) — agrupación en memoria
        var inicio = DateTime.UtcNow.Date.AddDays(-(DiasActividad - 1));
        var actividadSemana = Enumerable.Range(0, DiasActividad)
            .Select(i => inicio.AddDays(i))
            .Select(dia => new ActividadDiariaDto
            {
                Fecha    = dia.ToString("ddd dd/MM", new CultureInfo("es-ES")),
                Cantidad = fechas.Count(f => f.Date == dia)
            })
            .ToList();

        return new SistemaDashboardDto
        {
            UsuariosActivos   = usuarios.Count(u => u.Activo),
            CuentasPendientes = usuarios.Count(u => !u.Activo),
            AccionesHoy       = accionesHoy,
            AlertasSemana     = errores.Count(l => l.FechaHora >= hace7dias),

            ActividadSemana = actividadSemana,

            UsuariosPorRol = new[]
            {
                new UsuarioPorRolDto { Rol = "Administrador", Cantidad = usuarios.Count(u => u.RolId == (int)RolEnum.Administrador) },
                new UsuarioPorRolDto { Rol = "Médico",        Cantidad = usuarios.Count(u => u.RolId == (int)RolEnum.Medico) },
                new UsuarioPorRolDto { Rol = "Padre",         Cantidad = usuarios.Count(u => u.RolId == (int)RolEnum.Padre) },
            },

            AccionesRecientes = _mapper.Map<IEnumerable<AuditoriaDto.Response>>(recientes),
            AlertasRecientes  = _mapper.Map<IEnumerable<LogSistemaDto.Response>>(errores.Take(5)),
        };
    }
}
