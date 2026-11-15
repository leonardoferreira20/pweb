using RESTfulAPI.DTOs;

namespace RESTfulAPI.Services;

// O que os controllers podem pedir sobre modos de disponibilização. Recebe e devolve DTOs.
// O mesmo desenho do ICategoriaService.
public interface IModoDisponibilizacaoService
{
    // GET /api/modos-disponibilizacao: a lista.
    Task<List<ModoDisponibilizacaoDto>> GetAllAsync();

    // GET /api/modos-disponibilizacao/{id}: o modo, ou NaoEncontrado.
    Task<Resultado<ModoDisponibilizacaoDto>> GetByIdAsync(int id);

    // POST /api/modos-disponibilizacao: o modo criado, ou Conflito (nome repetido).
    Task<Resultado<ModoDisponibilizacaoDto>> CreateAsync(CriarModoDisponibilizacaoDto dto);

    // PUT /api/modos-disponibilizacao/{id}: NaoEncontrado ou Conflito se falhar.
    Task<Resultado> UpdateAsync(int id, AtualizarModoDisponibilizacaoDto dto);

    // DELETE /api/modos-disponibilizacao/{id}: NaoEncontrado ou Conflito (em uso) se falhar.
    Task<Resultado> DeleteAsync(int id);
}
