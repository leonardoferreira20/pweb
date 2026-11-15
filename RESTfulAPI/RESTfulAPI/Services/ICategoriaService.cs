using RESTfulAPI.DTOs;

namespace RESTfulAPI.Services;

// O que os controllers podem pedir sobre categorias. Recebe e devolve DTOs, nunca entidades.
// Os métodos que podem falhar devolvem um Resultado: o controller vê se correu bem
// e, se não, traduz o erro num status code.
public interface ICategoriaService
{
    // GET /api/categorias: as categorias ativas (nunca falha; pode vir vazia).
    Task<List<CategoriaDto>> GetAllAsync();

    // GET /api/categorias/{id}: a categoria, ou NaoEncontrado.
    Task<Resultado<CategoriaDto>> GetByIdAsync(int id);

    // GET /api/categorias/{id}/alojamentos: os alojamentos da categoria, ou NaoEncontrado.
    Task<Resultado<List<AlojamentoDto>>> GetAlojamentosAsync(int id);

    // POST /api/categorias: a categoria criada (já com Id), ou Conflito (nome repetido).
    Task<Resultado<CategoriaDto>> CreateAsync(CriarCategoriaDto dto);

    // PUT /api/categorias/{id}: sem valor; NaoEncontrado ou Conflito se falhar.
    Task<Resultado> UpdateAsync(int id, AtualizarCategoriaDto dto);

    // DELETE /api/categorias/{id}: sem valor; NaoEncontrado ou Conflito se falhar.
    Task<Resultado> DeleteAsync(int id);
}
