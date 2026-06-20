namespace SagradaFamilia.Application.DTOs
{
    public class SistemaDashboardDto
    {
        // ── Métricas de estado ────────────────────────────────────
        public int UsuariosActivos    { get; set; }
        public int CuentasPendientes  { get; set; } // cuentas sin activar
        public int AccionesHoy        { get; set; } // entradas de auditoría del día
        public int AlertasSemana      { get; set; } // errores en los últimos 7 días

        // ── Gráficos ─────────────────────────────────────────────
        public IEnumerable<ActividadDiariaDto> ActividadSemana  { get; set; } = [];
        public IEnumerable<UsuarioPorRolDto>   UsuariosPorRol   { get; set; } = [];

        // ── Tablas ───────────────────────────────────────────────
        public IEnumerable<AuditoriaDto.Response>  AccionesRecientes { get; set; } = [];
        public IEnumerable<LogSistemaDto.Response> AlertasRecientes  { get; set; } = [];
    }

    public class ActividadDiariaDto
    {
        public string Fecha    { get; set; } = string.Empty; // "Lun 14/06"
        public int    Cantidad { get; set; }
    }

    public class UsuarioPorRolDto
    {
        public string Rol      { get; set; } = string.Empty;
        public int    Cantidad { get; set; }
    }
}
