using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Seed
{
    public static class MedicoSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // RolId 1 = Medico (definido en RolAccionSeeder)
            if (await context.Usuarios.AnyAsync(u => u.RolId == 1))
                return;

            context.Usuarios.Add(new Usuario
            {
                Nombre = "Dr. Administrador",
                Apellido = "Sistema",
                Email = "medico@sagrada.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123"),
                RolId = 1,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
        }
    }
}
