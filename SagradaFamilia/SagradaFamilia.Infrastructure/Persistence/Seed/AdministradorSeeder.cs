using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Seed
{
    public static class AdministradorSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            int idRolAdmin = (int)RolEnum.Administrador;

            if (await context.Usuarios.AnyAsync(u => u.RolId == idRolAdmin))
                return;

            var usuarioAdmin = new Usuario
            {
                Email = "admin@sagrada.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123"),
                RolId = idRolAdmin,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            context.Usuarios.Add(usuarioAdmin);
            await context.SaveChangesAsync();
        }
    }
}