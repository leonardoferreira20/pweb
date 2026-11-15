using Microsoft.AspNetCore.Mvc;
using RESTfulAPI.DTOs;
using RESTfulAPI.Services;

namespace RESTfulAPI.Controllers;

// Os endpoints de /api/categorias. Cada método: recebe o pedido (já em DTO e validado),
// chama UM método do Service, e escolhe o status code da resposta.
// [ApiController]: valida os DTOs antes do método (400 automático), e lê o body como JSON.
// [Route]: o início do endereço de todos os métodos desta classe.
// (ICategoriaService service): o ASP.NET Core passa o service pelo construtor (injeção de dependências).
/// <summary>As categorias (tipologias) dos alojamentos.</summary>
[ApiController]
[Route("api/categorias")]
public class CategoriasController(ICategoriaService service) : ApiControllerBase
{
    // GET /api/categorias
    // Ok(...) = 200, com a lista em JSON no body. Nunca falha: no pior caso, a lista vem vazia.
    /// <summary>Lista as categorias ativas, pelo nome.</summary>
    /// <response code="200">A lista (pode vir vazia).</response>
    [HttpGet]
    public async Task<ActionResult<List<CategoriaDto>>> GetAll()
    {
        return Ok(await service.GetAllAsync());
    }

    // GET /api/categorias/3
    // "{id:int}": o id vem do URL e tem de ser um número (/api/categorias/abc dá 404).
    // [ProducesResponseType]: só documenta no Swagger que este endpoint pode responder 404.
    /// <summary>Uma categoria, pelo id (ativa ou não).</summary>
    /// <param name="id">O id da categoria.</param>
    /// <response code="200">A categoria.</response>
    /// <response code="404">CATEGORIA_NAO_ENCONTRADA.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaDto>> GetById(int id)
    {
        // O padrão de todos os métodos: chamar o Service; se falhou, Erro(r); se não, a resposta.
        var r = await service.GetByIdAsync(id);
        if (!r.Sucesso) return Erro(r);
        return Ok(r.Valor);
    }

    // GET /api/categorias/3/alojamentos
    // Um sub-recurso: os alojamentos que pertencem a esta categoria.
    /// <summary>Os alojamentos (ativos) de uma categoria.</summary>
    /// <param name="id">O id da categoria.</param>
    /// <response code="200">Os alojamentos (pode vir vazia).</response>
    /// <response code="404">CATEGORIA_NAO_ENCONTRADA.</response>
    [HttpGet("{id:int}/alojamentos")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<AlojamentoDto>>> GetAlojamentos(int id)
    {
        var r = await service.GetAlojamentosAsync(id);
        if (!r.Sucesso) return Erro(r);
        return Ok(r.Valor);
    }

    // POST /api/categorias
    // O parâmetro dto vem do body do pedido: o ASP.NET Core converte o JSON num CriarCategoriaDto.
    /// <summary>Cria uma categoria.</summary>
    /// <param name="dto">A categoria nova (sem id).</param>
    /// <response code="201">A categoria criada; o header Location diz onde.</response>
    /// <response code="400">DADOS_INVALIDOS ou JSON_INVALIDO.</response>
    /// <response code="409">CATEGORIA_NOME_REPETIDO.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CategoriaDto>> Create(CriarCategoriaDto dto)
    {
        var r = await service.CreateAsync(dto);
        if (!r.Sucesso) return Erro(r);
        // 201 Created + header Location: /api/categorias/{id} + a categoria criada no body.
        return CreatedAtAction(nameof(GetById), new { id = r.Valor!.Id }, r.Valor);
    }

    // PUT /api/categorias/3
    // Dois parâmetros: o id vem do URL, o dto vem do body.
    /// <summary>Altera uma categoria: manda-se a categoria inteira.</summary>
    /// <param name="id">O id da categoria.</param>
    /// <param name="dto">Os novos valores.</param>
    /// <response code="204">Alterada (sem body).</response>
    /// <response code="400">DADOS_INVALIDOS ou JSON_INVALIDO.</response>
    /// <response code="404">CATEGORIA_NAO_ENCONTRADA.</response>
    /// <response code="409">CATEGORIA_NOME_REPETIDO.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, AtualizarCategoriaDto dto)
    {
        var r = await service.UpdateAsync(id, dto);
        if (!r.Sucesso) return Erro(r);
        // 204 No Content: correu bem, e não há nada para devolver.
        return NoContent();
    }

    // DELETE /api/categorias/3
    // 409 se a categoria tiver alojamentos (regra do CategoriaService).
    /// <summary>Apaga uma categoria, se não tiver alojamentos.</summary>
    /// <param name="id">O id da categoria.</param>
    /// <response code="204">Apagada (sem body).</response>
    /// <response code="404">CATEGORIA_NAO_ENCONTRADA.</response>
    /// <response code="409">CATEGORIA_COM_ALOJAMENTOS.</response>
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
