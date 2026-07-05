using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SagradaFamilia.Domain.Common;
using SagradaFamilia.Domain.Entities;
using System.Security.Claims;
using System.Text.Json;

namespace SagradaFamilia.Infrastructure.Persistence.Contexts
{
    public class AppDbContext : DbContext
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor? httpContextAccessor = null)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Constructor para clases derivadas (PostgresAppDbContext, etc.)
        protected AppDbContext(DbContextOptions options, IHttpContextAccessor? httpContextAccessor = null)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // ── Seguridad y menú ─────────────────────────────────────
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Modulo> Modulos => Set<Modulo>();
        public DbSet<Opcion> Opciones => Set<Opcion>();
        public DbSet<OpcionAccion> OpcionAcciones => Set<OpcionAccion>();
        public DbSet<RolPermiso> RolPermisos => Set<RolPermiso>();
        public DbSet<UsuarioPermiso> UsuarioPermisos => Set<UsuarioPermiso>();
        public DbSet<Rol> Roles => Set<Rol>();
        public DbSet<Accion> Acciones => Set<Accion>();

        // ── Perfiles ─────────────────────────────────────────────
        public DbSet<Padre> Padres => Set<Padre>();
        public DbSet<Medico> Medicos => Set<Medico>();

        // ── Clínico ──────────────────────────────────────────────
        public DbSet<Nino> Ninos => Set<Nino>();
        public DbSet<Medida> Medidas => Set<Medida>();
        public DbSet<Prediccion> Predicciones => Set<Prediccion>();
        public DbSet<Cita> Citas => Set<Cita>();
        public DbSet<Consulta> Consultas => Set<Consulta>();
        public DbSet<Prescripcion> Prescripciones => Set<Prescripcion>();
        public DbSet<Medicamento> Medicamentos => Set<Medicamento>();

        // ── OMS ──────────────────────────────────────────────────
        public DbSet<OmsReferencia> OmsReferencias => Set<OmsReferencia>();

        // ── Alimentos ────────────────────────────────────────────
        public DbSet<CategoriaAlimento> CategoriasAlimentos => Set<CategoriaAlimento>();
        public DbSet<Alimento> Alimentos => Set<Alimento>();

        // ── Sistema y Trazabilidad ───────────────────────────────
        public DbSet<LogSistema> LogsSistema => Set<LogSistema>();
        public DbSet<Auditoria> Auditorias => Set<Auditoria>();
        public DbSet<ParametroSistema> ParametrosSistema => Set<ParametroSistema>();
        public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
        public DbSet<PlantillaCorreo> PlantillasCorreo => Set<PlantillaCorreo>();
        public DbSet<EventoCorreo> EventosCorreo => Set<EventoCorreo>();

        // ── Tipos excluidos de auditoría ─────────────────────────
        private static readonly HashSet<Type> _tiposNoAuditados = new()
        {
            typeof(Auditoria),
            typeof(LogSistema),
            typeof(RefreshToken),
            typeof(PasswordResetToken),
            typeof(OmsReferencia)
        };

        // Propiedades excluidas del snapshot de valores (datos sensibles)
        private static readonly HashSet<string> _propiedadesExcluidas =
            new(StringComparer.OrdinalIgnoreCase) { "PasswordHash", "Token" };

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // 1. Soft delete + timestamps (lógica existente)
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.FechaCreacion = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.FechaActualizacion = DateTime.UtcNow;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.Eliminado = true;
                        entry.Entity.FechaEliminacion = DateTime.UtcNow;
                        break;
                }
            }

            // 2. Capturar entradas de auditoría ANTES del save
            //    (OriginalValues solo están disponibles antes del INSERT/UPDATE)
            var usuarioId = ObtenerUsuarioActualId();
            var ip = _httpContextAccessor?.HttpContext?.Connection.RemoteIpAddress?.ToString();
            var pendientes = CapturarAntesDeGuardar();

            // 3. Guardar los cambios principales
            var resultado = await base.SaveChangesAsync(cancellationToken);

            // 4. Completar y persistir registros de auditoría
            //    (ahora los PKs autogenerados de Added ya están disponibles)
            if (pendientes.Count > 0 && usuarioId.HasValue)
            {
                var entradas = CompletarDespuesDeGuardar(pendientes, usuarioId.Value, ip);
                if (entradas.Count > 0)
                {
                    Auditorias.AddRange(entradas);
                    // Llamamos a base directamente para evitar recursión
                    await base.SaveChangesAsync(cancellationToken);
                }
            }

            return resultado;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        // ── Helpers de auditoría ─────────────────────────────────

        private int? ObtenerUsuarioActualId()
        {
            var claim = _httpContextAccessor?.HttpContext?.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }

        private sealed record AuditoriaPendiente(
            EntityEntry Entry,
            string Tabla,
            string Accion,
            string? ValoresAntiguos,
            string? ValoresNuevos,
            string? ClavePrimaria  // null cuando el estado era Added (PK aún no asignado)
        );

        private List<AuditoriaPendiente> CapturarAntesDeGuardar()
        {
            var pendientes = new List<AuditoriaPendiente>();

            foreach (var entry in ChangeTracker.Entries())
            {
                if (_tiposNoAuditados.Contains(entry.Entity.GetType())) continue;
                if (entry.State is EntityState.Detached or EntityState.Unchanged) continue;

                string accion;
                string? valoresAntiguos = null;
                string? valoresNuevos = null;
                string? clavePrimaria = null;

                if (entry.State == EntityState.Added)
                {
                    accion = "Creación";
                    valoresNuevos = Serializar(entry.CurrentValues);
                    // clavePrimaria queda null — se resuelve después del INSERT
                }
                else if (entry.State == EntityState.Deleted)
                {
                    accion = "Eliminación";
                    valoresAntiguos = Serializar(entry.OriginalValues);
                    clavePrimaria = ObtenerPk(entry);
                }
                else // Modified
                {
                    // Detectar si es eliminación lógica (Eliminado cambió de false a true)
                    var propEliminado = entry.Properties
                        .FirstOrDefault(p => p.Metadata.Name == "Eliminado");

                    accion = propEliminado is { CurrentValue: true, OriginalValue: false }
                        ? "Eliminación"
                        : "Actualización";

                    valoresAntiguos = Serializar(entry.OriginalValues);
                    valoresNuevos = Serializar(entry.CurrentValues);
                    clavePrimaria = ObtenerPk(entry);
                }

                pendientes.Add(new AuditoriaPendiente(
                    entry, entry.Entity.GetType().Name,
                    accion, valoresAntiguos, valoresNuevos, clavePrimaria));
            }

            return pendientes;
        }

        private static List<Auditoria> CompletarDespuesDeGuardar(
            List<AuditoriaPendiente> pendientes, int usuarioId, string? ip)
        {
            var entradas = new List<Auditoria>();

            foreach (var p in pendientes)
            {
                // Para Added, ahora el PK ya fue asignado por la BD
                var pk = p.ClavePrimaria ?? ObtenerPk(p.Entry);

                entradas.Add(new Auditoria
                {
                    UsuarioId = usuarioId,
                    Fecha = DateTime.UtcNow,
                    Accion = p.Accion,
                    Tabla = p.Tabla,
                    ClavePrimaria = pk,
                    ValoresAntiguos = p.ValoresAntiguos,
                    ValoresNuevos = p.ValoresNuevos,
                    IpAddress = ip
                });
            }

            return entradas;
        }

        private static string ObtenerPk(EntityEntry entry) =>
            entry.Properties
                .FirstOrDefault(p => p.Metadata.IsPrimaryKey())
                ?.CurrentValue?.ToString() ?? "0";

        private static string Serializar(PropertyValues values)
        {
            var dict = new Dictionary<string, object?>();

            foreach (var prop in values.Properties)
            {
                if (_propiedadesExcluidas.Contains(prop.Name)) continue;
                dict[prop.Name] = values[prop];
            }

            return JsonSerializer.Serialize(dict, new JsonSerializerOptions
            {
                WriteIndented = false,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
        }
    }
}
