using System.ComponentModel.DataAnnotations;

namespace SagradaFamilia.Application.DTOs.Auth;

public class SolicitarResetDto
{
    public record Request(
        [Required, EmailAddress] string Email
    );
}
