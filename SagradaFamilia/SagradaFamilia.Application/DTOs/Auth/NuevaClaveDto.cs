using System.ComponentModel.DataAnnotations;

namespace SagradaFamilia.Application.DTOs.Auth;

public class NuevaClaveDto
{
    public record Request(
        [Required] string Token,
        [Required, MinLength(8)] string NuevaClave
    );
}
