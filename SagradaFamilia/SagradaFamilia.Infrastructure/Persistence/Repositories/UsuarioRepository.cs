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

        public async Task<Usuario?> ObtenerConRolesYPermisosAsync(int id) =>
            await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Permisos)
                    .ThenInclude(up => up.OpcionAccion)
                .FirstOrDefaultAsync(u => u.Id == id && !u.Eliminado && u.Activo);

        public async Task<bool> ExisteEmailAsync(string email) =>
            await _context.Usuarios
                .AnyAsync(u => u.Email == email && !u.Eliminado);

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

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }
    }
}