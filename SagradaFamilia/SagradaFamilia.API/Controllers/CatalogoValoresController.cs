namespace SagradaFamilia.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;
using SagradaFamilia.Application.Interfaces.Services;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CatalogoValoresController : BaseController
{
    private readonly ICatalogoValorService _catalogoValorService;

    public CatalogoValoresController(ICatalogoValorService catalogoValorService) =>
        _catalogoValorService = catalogoValorService;

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CatalogoValorDto.Response>>>> ObtenerTodos()
    {
        var data = await _catalogoValorService.ObtenerTodosAsync();
        return HandleResponse(data);
    }

    [HttpGet("tipo/{tipo}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CatalogoValorDto.Response>>>> ObtenerPorTipo(string tipo)
    {
        var data = await _catalogoValorService.ObtenerPorTipoAsync(tipo);
        return HandleResponse(data);
    }

    [HttpGet("tipo/{tipo}/codigo/{codigo}")]
    public async Task<ActionResult<ApiResponse<CatalogoValorDto.Response?>>> ObtenerPorTipoYCodigo(string tipo, string codigo)
    {
        var data = await _catalogoValorService.ObtenerPorTipoYCodigoAsync(tipo, codigo);
        return HandleResponse(data);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<CatalogoValorDto.Response>>> ObtenerPorId(int id)
    {
        var data = await _catalogoValorService.ObtenerPorIdAsync(id);
        return HandleResponse(data);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse<CatalogoValorDto.Response>>> Crear([FromBody] CatalogoValorDto.Create request)
    {
        var data = await _catalogoValorService.CrearAsync(request);
        return HandleResponse(data, "Valor de catálogo creado exitosamente.");
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse<CatalogoValorDto.Response>>> Actualizar(int id, [FromBody] CatalogoValorDto.Update request)
    {
        var data = await _catalogoValorService.ActualizarAsync(id, request);
        return HandleResponse(data, "Valor de catálogo actualizado exitosamente.");
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse>> Eliminar(int id)
    {
        await _catalogoValorService.EliminarAsync(id);
        return HandleSuccess("Valor de catálogo eliminado exitosamente.");
    }
}
