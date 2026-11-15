using Microsoft.AspNetCore.Mvc;
using RESTfulAPI.DTOs;
using RESTfulAPI.Services;

namespace RESTfulAPI.Controllers;

// Os endpoints de /api/hoteis. Só GET, nesta etapa: as unidades gerem-se na Gestão de Hotéis.
/// <summary>As unidades hoteleiras (hotéis e aparthotéis). Só leitura.</summary>
[ApiController]
[Route("api/hoteis")]
public class HoteisController(IHotelService service) : ApiControllerBase
{
    /// <summary>Lista as unidades ativas, pelo nome, com a morada.</summary>
    /// <response code="200">A lista (pode vir vazia).</response>
    [HttpGet]
    public async Task<ActionResult<List<HotelDto>>> GetAll()
    {
        return Ok(await service.GetAllAsync());
    }

    /// <summary>Uma unidade hoteleira, pelo id (ativa ou não).</summary>
    /// <param name="id">O id da unidade.</param>
    /// <response code="200">A unidade.</response>
    /// <response code="404">HOTEL_NAO_ENCONTRADO.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HotelDto>> GetById(int id)
    {
        var r = await service.GetByIdAsync(id);
        if (!r.Sucesso) return Erro(r);
        return Ok(r.Valor);
    }

    // GET /api/hoteis/1/alojamentos: um sub-recurso, como os alojamentos de uma categoria.
    /// <summary>Os alojamentos (ativos) de uma unidade hoteleira.</summary>
    /// <param name="id">O id da unidade.</param>
    /// <response code="200">Os alojamentos (vazia se a unidade estiver inativa).</response>
    /// <response code="404">HOTEL_NAO_ENCONTRADO.</response>
    [HttpGet("{id:int}/alojamentos")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<AlojamentoDto>>> GetAlojamentos(int id)
    {
        var r = await service.GetAlojamentosAsync(id);
        if (!r.Sucesso) return Erro(r);
        return Ok(r.Valor);
    }
}
