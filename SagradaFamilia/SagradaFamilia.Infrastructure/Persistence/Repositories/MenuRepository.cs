using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;
using SagradaFamilia.Domain.Interfaces.Repositories;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly AppDbContext _context;

        public MenuRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Modulo>> ObtenerModulosActivosAsync() =>
            await _context.Modulos
                .Include(m => m.Opciones.Where(o => o.Activo && !o.Eliminado))
                    .ThenInclude(o => o.OpcionAcciones)
                        .ThenInclude(oa => oa.Accion)
                .Where(m => m.Activo && !m.Eliminado)
                .OrderBy(m => m.Orden)
                .ToListAsync();

        public async Task<Modulo?> ObtenerModuloPorIdAsync(int id) =>
            await _context.Modulos
                .Include(m => m.Opciones)
                .FirstOrDefaultAsync(m => m.Id == id && !m.Eliminado);

        public async Task<Opcion?> ObtenerOpcionPorIdAsync(int id) =>
            await _context.Opciones
                .Include(o => o.OpcionAcciones)
                    .ThenInclude(oa => oa.Accion)
                .FirstOrDefaultAsync(o => o.Id == id && !o.Eliminado);

        public async Task<OpcionAccion?> ObtenerOpcionAccionAsync(int opcionId, int accionId) =>
            await _context.OpcionAcciones
                .FirstOrDefaultAsync(oa => oa.OpcionId == opcionId
                                        && oa.AccionId == accionId);

        public async Task<IEnumerable<RolPermiso>> ObtenerPermisosPorRolIdAsync(int rolId) =>
            await _context.RolPermisos
                .Include(rp => rp.OpcionAccion)
                    .ThenInclude(oa => oa.Opcion)
                        .ThenInclude(o => o.Modulo)
                .Include(rp => rp.OpcionAccion)
                    .ThenInclude(oa => oa.Accion)
                .Where(rp => rp.RolId == rolId && rp.Permitido)
                .ToListAsync();

        public async Task<IEnumerable<UsuarioPermiso>> ObtenerPermisosPorUsuarioAsync(int usuarioId) =>
            await _context.UsuarioPermisos
                .Include(up => up.OpcionAccion)
                    .ThenInclude(oa => oa.Opcion)
                        .ThenInclude(o => o.Modulo)
                .Include(up => up.OpcionAccion)
                    .ThenInclude(oa => oa.Accion)
                .Where(up => up.UsuarioId == usuarioId)
                .ToListAsync();

        public async Task<UsuarioPermiso> CrearUsuarioPermisoAsync(UsuarioPermiso permiso)
        {
            _context.UsuarioPermisos.Add(permiso);
            await _context.SaveChangesAsync();
            return permiso;
        }

        public async Task EliminarUsuarioPermisoAsync(int usuarioId, int opcionAccionId)
        {
            var permiso = await _context.UsuarioPermisos
                .FirstOrDefaultAsync(up => up.UsuarioId == usuarioId
                                        && up.OpcionAccionId == opcionAccionId);

            if (permiso is null) return;

            _context.UsuarioPermisos.Remove(permiso);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Modulo>> ObtenerMenuPorUsuarioAsync(int usuarioId, int rolId)
        {
            // 1. Permisos base del rol
            var permisosRol = await _context.RolPermisos
                .Where(rp => rp.RolId == rolId && rp.Permitido)
                .Select(rp => rp.OpcionAccionId)
                .ToListAsync();

            // 2. Permisos individuales del usuario
            var permisosUsuario = await _context.UsuarioPermisos
                .Where(up => up.UsuarioId == usuarioId)
                .ToListAsync();

            // 3. Combinar — permisos individuales pueden agregar o quitar acceso
            var agregados = permisosUsuario
                .Where(up => up.Permitido)
                .Select(up => up.OpcionAccionId);

            var quitados = permisosUsuario
                .Where(up => !up.Permitido)
                .Select(up => up.OpcionAccionId);

            var opcionAccionesPermitidas = permisosRol
                .Union(agregados)
                .Except(quitados)
                .ToList();

            // 4. Traer menú filtrado desde la base de datos
            var modulos = await _context.Modulos
                .Include(m => m.Opciones.Where(o => o.Activo && !o.Eliminado))
                    .ThenInclude(o => o.OpcionAcciones
                        .Where(oa => opcionAccionesPermitidas.Contains(oa.Id)))
                        .ThenInclude(oa => oa.Accion)
                .Where(m => m.Activo && !m.Eliminado)
                .OrderBy(m => m.Orden)
                .ToListAsync();

            // 5. NUEVO: Limpiar en memoria las opciones que se quedaron sin acciones
            foreach (var modulo in modulos)
            {
                modulo.Opciones = modulo.Opciones
                    .Where(o => o.OpcionAcciones.Any())
                    .ToList();
            }

            // 6. Retornar solo los módulos que tienen opciones válidas
            return modulos.Where(m => m.Opciones.Any());
        }
    }
}
