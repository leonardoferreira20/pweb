using Microsoft.AspNetCore.Mvc;
using RESTfulAPI.DTOs;
using RESTfulAPI.Services;

namespace RESTfulAPI.Controllers;

// Os endpoints de /api/modos-disponibilizacao (minúsculas e hífen: boa prática nos URLs).
// O mesmo desenho do CategoriasController: cada método chama um método do Service
// e escolhe o status code (Ok 200, CreatedAtAction 201, NoContent 204, ou Erro).
/// <summary>Os modos de disponibilização (Listagem e Reserva).</summary>
[ApiController]
[Route("api/modos-disponibilizacao")]
public class ModosDisponibilizacaoController(IModoDisponibilizacaoService service) : ApiControllerBase
{
    // GET /api/modos-disponibilizacao → 200 com a lista.
    /// <summary>Lista os modos de disponibilização, pelo nome.</summary>
    /// <response code="200">A lista (pode vir vazia).</response>
    [HttpGet]
    public async Task<ActionResult<List<ModoDisponibilizacaoDto>>> GetAll()
    {
        return Ok(await service.GetAllAsync());
    }

    // GET /api/modos-disponibilizacao/2 → 200 com o modo, ou 404.
    /// <summary>Um modo de disponibilização, pelo id.</summary>
    /// <param name="id">O id do modo.</param>
    /// <response code="200">O modo.</response>
    /// <response code="404">MODO_NAO_ENCONTRADO.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ModoDisponibilizacaoDto>> GetById(int id)
    {
        var r = await service.GetByIdAsync(id);
        if (!r.Sucesso) return Erro(r);
        return Ok(r.Valor);
    }

    // POST /api/modos-disponibilizacao → 201 com o header Location, 400 ou 409.
    /// <summary>Cria um modo de disponibilização.</summary>
    /// <param name="dto">O modo novo (sem id).</param>
    /// <response code="201">O modo criado; o header Location diz onde.</response>
    /// <response code="400">DADOS_INVALIDOS ou JSON_INVALIDO.</response>
    /// <response code="409">MODO_NOME_REPETIDO.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ModoDisponibilizacaoDto>> Create(CriarModoDisponibilizacaoDto dto)
    {
        var r = await service.CreateAsync(dto);
        if (!r.Sucesso) return Erro(r);
        return CreatedAtAction(nameof(GetById), new { id = r.Valor!.Id }, r.Valor);
    }

    // PUT /api/modos-disponibilizacao/2 → 204, 400, 404 ou 409.
    /// <summary>Altera um modo de disponibilização.</summary>
    /// <param name="id">O id do modo.</param>
    /// <param name="dto">Os novos valores.</param>
    /// <response code="204">Alterado (sem body).</response>
    /// <response code="400">DADOS_INVALIDOS ou JSON_INVALIDO.</response>
    /// <response code="404">MODO_NAO_ENCONTRADO.</response>
    /// <response code="409">MODO_NOME_REPETIDO.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, AtualizarModoDisponibilizacaoDto dto)
    {
        var r = await service.UpdateAsync(id, dto);
        if (!r.Sucesso) return Erro(r);
        return NoContent();
    }

    // DELETE /api/modos-disponibilizacao/2 → 204, 404, ou 409 se houver alojamentos com este modo.
    /// <summary>Apaga um modo de disponibilização, se não estiver em uso.</summary>
    /// <param name="id">O id do modo.</param>
    /// <response code="204">Apagado (sem body).</response>
    /// <response code="404">MODO_NAO_ENCONTRADO.</response>
    /// <response code="409">MODO_EM_USO.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await service.DeleteAsync(id);
        if (!r.Sucesso) return Erro(r);
        return NoContent();
    }
}
