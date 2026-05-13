using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SagradaFamilia.Domain.Common;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

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
        public DbSet<Prescripcion> Prescripciones => Set<Prescripcion>();

        // ── OMS ──────────────────────────────────────────────────
        public DbSet<OmsReferencia> OmsReferencias => Set<OmsReferencia>();

        // ── Alimentos ────────────────────────────────────────────
        public DbSet<CategoriaAlimento> CategoriasAlimentos => Set<CategoriaAlimento>();
        public DbSet<Alimento> Alimentos => Set<Alimento>();

        // ── Sistema y Trazabilidad ───────────────────────────────
        public DbSet<LogSistema> LogsSistema => Set<LogSistema>();
        public DbSet<Auditoria> Auditorias => Set<Auditoria>();

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
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

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}