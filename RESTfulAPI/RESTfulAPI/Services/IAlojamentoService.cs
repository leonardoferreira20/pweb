using RESTfulAPI.DTOs;

namespace RESTfulAPI.Services;

// O que os controllers podem pedir sobre alojamentos. Recebe e devolve DTOs.
public interface IAlojamentoService
{
    // GET /api/alojamentos: a lista, com os filtros que vierem do URL.
    Task<List<AlojamentoDto>> GetAllAsync(int? hotelId, int? categoriaId, int? estrelas, string? localidade, string? pesquisa);

    // GET /api/alojamentos/{id}: o alojamento, ou NaoEncontrado.
    Task<Resultado<AlojamentoDto>> GetByIdAsync(int id);

    // POST /api/alojamentos: o alojamento criado; Invalido (referência ou imagem) ou Conflito.
    Task<Resultado<AlojamentoDto>> CreateAsync(CriarAlojamentoDto dto);

    // PUT /api/alojamentos/{id}: NaoEncontrado, Invalido ou Conflito se falhar.
    Task<Resultado> UpdateAsync(int id, AtualizarAlojamentoDto dto);

    // DELETE /api/alojamentos/{id}: NaoEncontrado ou Conflito (com reservas) se falhar.
    Task<Resultado> DeleteAsync(int id);
}
