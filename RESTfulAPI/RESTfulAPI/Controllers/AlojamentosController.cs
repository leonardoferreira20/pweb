using Microsoft.AspNetCore.Mvc;
using RESTfulAPI.DTOs;
using RESTfulAPI.Services;

namespace RESTfulAPI.Controllers;

// Os endpoints de /api/alojamentos.
// O mesmo desenho dos outros controllers: cada método chama UM método do Service
// e escolhe o status code. Não tem regras de negócio nem acesso à base de dados.
/// <summary>Os alojamentos (tipologias de quarto ou apartamento) das unidades hoteleiras.</summary>
[ApiController]
[Route("api/alojamentos")]
public class AlojamentosController(IAlojamentoService service) : ApiControllerBase
{
    // GET /api/alojamentos?hotelId=1&categoriaId=2&estrelas=4&localidade=Porto&pesquisa=suite
    // Os cinco filtros vêm da query string e são opcionais (RN19).
    // [FromQuery]: o valor vem do que está depois do "?" no URL; se não vier, fica null.
    /// <summary>Lista os alojamentos ativos de unidades ativas, com filtros opcionais.</summary>
    /// <param name="hotelId">Só os desta unidade hoteleira.</param>
    /// <param name="categoriaId">Só os desta categoria.</param>
    /// <param name="estrelas">Só os de unidades com esta classificação (1 a 5).</param>
    /// <param name="localidade">Texto a procurar na localidade da unidade.</param>
    /// <param name="pesquisa">Texto a procurar no nome ou na descrição.</param>
    /// <response code="200">A lista (pode vir vazia).</response>
    /// <response code="400">DADOS_INVALIDOS: um filtro com um valor que não serve (por exemplo estrelas=abc).</response>
    [HttpGet]
    public async Task<ActionResult<List<AlojamentoDto>>> GetAll(
        [FromQuery] int? hotelId,
        [FromQuery] int? categoriaId,
        [FromQuery] int? estrelas,
        [FromQuery] string? localidade,
        [FromQuery] string? pesquisa)
    {
        return Ok(await service.GetAllAsync(hotelId, categoriaId, estrelas, localidade, pesquisa));
    }

    // GET /api/alojamentos/7 → 200 com o alojamento (e as referências aninhadas), ou 404.
    /// <summary>Um alojamento, pelo id (ativo ou não), com a unidade, a categoria, o modo, os regimes e as características.</summary>
    /// <param name="id">O id do alojamento.</param>
    /// <response code="200">O alojamento.</response>
    /// <response code="404">ALOJAMENTO_NAO_ENCONTRADO.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlojamentoDto>> GetById(int id)
    {
        var r = await service.GetByIdAsync(id);
        if (!r.Sucesso) return Erro(r);
        return Ok(r.Valor);
    }

    // POST /api/alojamentos → 201 + Location, 400 (validação ou referência inexistente) ou 409.
    // Se o JSON tiver erros de validação, o [ApiController] responde 400 e este método nem corre.
    /// <summary>Cria um alojamento, com a imagem em Base64.</summary>
    /// <param name="dto">O alojamento novo (sem id).</param>
    /// <response code="201">O alojamento criado; o header Location diz onde.</response>
    /// <response code="400">DADOS_INVALIDOS, JSON_INVALIDO, HOTEL_INEXISTENTE, CATEGORIA_INEXISTENTE, MODO_INEXISTENTE, REGIME_INEXISTENTE, CARACTERISTICA_INEXISTENTE ou IMAGEM_...</response>
    /// <response code="409">ALOJAMENTO_NOME_REPETIDO.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AlojamentoDto>> Create(CriarAlojamentoDto dto)
    {
        var r = await service.CreateAsync(dto);
        if (!r.Sucesso) return Erro(r);
        // 201 Created, header Location: /api/alojamentos/{id}, e o alojamento criado no body.
        return CreatedAtAction(nameof(GetById), new { id = r.Valor!.Id }, r.Valor);
    }

    // PUT /api/alojamentos/7 → 204, 400, 404 ou 409.
    /// <summary>Altera um alojamento: manda-se o alojamento inteiro, com as listas de regimes e de características e a imagem.</summary>
    /// <param name="id">O id do alojamento.</param>
    /// <param name="dto">Os novos valores.</param>
    /// <response code="204">Alterado (sem body).</response>
    /// <response code="400">DADOS_INVALIDOS, JSON_INVALIDO, ..._INEXISTENTE ou IMAGEM_...</response>
    /// <response code="404">ALOJAMENTO_NAO_ENCONTRADO.</response>
    /// <response code="409">ALOJAMENTO_NOME_REPETIDO.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, AtualizarAlojamentoDto dto)
    {
        var r = await service.UpdateAsync(id, dto);
        if (!r.Sucesso) return Erro(r);
        return NoContent();
    }

    // DELETE /api/alojamentos/7 → 204, 404, ou 409 se tiver reservas.
    /// <summary>Apaga um alojamento, se não tiver reservas.</summary>
    /// <param name="id">O id do alojamento.</param>
    /// <response code="204">Apagado (sem body).</response>
    /// <response code="404">ALOJAMENTO_NAO_ENCONTRADO.</response>
    /// <response code="409">ALOJAMENTO_COM_RESERVAS.</response>
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
