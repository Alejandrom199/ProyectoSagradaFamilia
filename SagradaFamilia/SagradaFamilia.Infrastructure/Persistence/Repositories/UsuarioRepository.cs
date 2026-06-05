using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Interfaces.Repositories;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context) => _context = context;

        public async Task<Usuario?> ObtenerPorIdAsync(int id) =>
            await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Padre)
                .Include(u => u.Medico)
                .FirstOrDefaultAsync(u => u.Id == id && !u.Eliminado);

        public async Task<Usuario?> ObtenerPorEmailAsync(string email) =>
            await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Padre)
                .Include(u => u.Medico)
                .FirstOrDefaultAsync(u => u.Email == email && !u.Eliminado);

        public async Task<IEnumerable<Usuario>> ObtenerPorRolIdAsync(int rolId) =>
            await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Padre)
                .Include(u => u.Medico)
                .Where(u => u.RolId == rolId && !u.Eliminado)
                .ToListAsync();

        public async Task<IEnumerable<Usuario>> ObtenerTodosAsync() =>
            await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Padre)
                .Include(u => u.Medico)
                .Where(u => !u.Eliminado)
                .ToListAsync();

        public async Task<(IEnumerable<Usuario> Items, int TotalItems)> ObtenerPaginadoAsync(
            int page, int pageSize, string? search, string? sortBy, bool ascending)
        {
            var query = _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Padre)
                .Include(u => u.Medico)
                .Where(u => !u.Eliminado)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(u =>
                    u.Email.ToLower().Contains(term) ||
                    u.Rol.Nombre.ToLower().Contains(term));
            }

            query = sortBy?.ToLower() switch
            {
                "email"         => ascending ? query.OrderBy(u => u.Email)            : query.OrderByDescending(u => u.Email),
                "rolnombre"     => ascending ? query.OrderBy(u => u.Rol.Nombre)       : query.OrderByDescending(u => u.Rol.Nombre),
                "activo"        => ascending ? query.OrderBy(u => u.Activo)           : query.OrderByDescending(u => u.Activo),
                "fechacreacion" => ascending ? query.OrderBy(u => u.FechaCreacion)    : query.OrderByDescending(u => u.FechaCreacion),
                _               => query.OrderBy(u => u.Email)
            };

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<Usuario?> ObtenerConRolesYPermisosAsync(int id) =>
            await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Permisos)
                    .ThenInclude(up => up.OpcionAccion)
                .FirstOrDefaultAsync(u => u.Id == id && !u.Eliminado && u.Activo);

        public async Task<bool> ExisteEmailAsync(string email) =>
            await _context.Usuarios
                .AnyAsync(u => u.Email == email && !u.Eliminado);

        public async Task<int> ContarAdministradoresActivosAsync() =>
            await _context.Usuarios
                .Where(u => u.Rol.Nombre == "Administrador" && u.Activo && !u.Eliminado)
                .CountAsync();

        public async Task<Usuario> CrearAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<Usuario> ActualizarAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task EliminarAsync(int id)
        {
            var usuario = await ObtenerPorIdAsync(id);
            if (usuario is null) return;

            usuario.Eliminado = true;
            usuario.FechaEliminacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}