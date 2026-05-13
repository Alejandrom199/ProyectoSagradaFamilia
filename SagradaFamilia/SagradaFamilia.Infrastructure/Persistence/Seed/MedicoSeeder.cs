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
            int idRolMedico = (int)RolEnum.Medico;

            if (await context.Usuarios.AnyAsync(u => u.RolId == idRolMedico))
                return;

            var usuarioMedico = new Usuario
            {
                Email = "medico@sagrada.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123"),
                RolId = idRolMedico,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            context.Usuarios.Add(usuarioMedico);
            await context.SaveChangesAsync();

            context.Medicos.Add(new Medico
            {
                UsuarioId = usuarioMedico.Id,
                Nombre = "Saúl Alejandro",
                Apellido = "Maldonado",
                Telefono = "0999999999",
                Especialidad = "Pediatría General"
            });

            await context.SaveChangesAsync();
        }
    }
}