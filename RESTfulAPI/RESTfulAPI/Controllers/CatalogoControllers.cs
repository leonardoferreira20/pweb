using Microsoft.AspNetCore.Mvc;
using RESTfulAPI.DTOs;
using RESTfulAPI.Services;

namespace RESTfulAPI.Controllers;

// Os endpoints de /api/regimes-alimentares e /api/caracteristicas. Só GET: a app precisa deles
// para mostrar os filtros e para escolher os ids no POST e no PUT dos alojamentos.
// Dois controllers pequenos no mesmo ficheiro, com o mesmo desenho.
/// <summary>Os regimes alimentares. Só leitura.</summary>
[ApiController]
[Route("api/regimes-alimentares")]
public class RegimesAlimentaresController(IRegimeAlimentarService service) : ApiControllerBase
{
    /// <summary>Lista os regimes ativos, pela percentagem.</summary>
    /// <response code="200">A lista (pode vir vazia).</response>
    [HttpGet]
    public async Task<ActionResult<List<RegimeAlimentarDto>>> GetAll()
    {
        return Ok(await service.GetAllAsync());
    }

    /// <summary>Um regime, pelo id (ativo ou não).</summary>
    /// <param name="id">O id do regime.</param>
    /// <response code="200">O regime.</response>
    /// <response code="404">REGIME_NAO_ENCONTRADO.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RegimeAlimentarDto>> GetById(int id)
    {
        var r = await service.GetByIdAsync(id);
        if (!r.Sucesso) return Erro(r);
        return Ok(r.Valor);
    }
}

/// <summary>As características dos alojamentos. Só leitura.</summary>
[ApiController]
[Route("api/caracteristicas")]
public class CaracteristicasController(ICaracteristicaService service) : ApiControllerBase
{
    /// <summary>Lista as características ativas, pelo nome.</summary>
    /// <response code="200">A lista (pode vir vazia).</response>
    [HttpGet]
    public async Task<ActionResult<List<CaracteristicaDto>>> GetAll()
    {
        return Ok(await service.GetAllAsync());
    }

    /// <summary>Uma característica, pelo id (ativa ou não).</summary>
    /// <param name="id">O id da característica.</param>
    /// <response code="200">A característica.</response>
    /// <response code="404">CARACTERISTICA_NAO_ENCONTRADA.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CaracteristicaDto>> GetById(int id)
    {
        var r = await service.GetByIdAsync(id);
        if (!r.Sucesso) return Erro(r);
        return Ok(r.Valor);
    }
}
