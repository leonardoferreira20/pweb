using RESTfulAPI.Entities;

namespace RESTfulAPI.Repositories;

// O que se pode pedir à base de dados sobre modos de disponibilização.
// Os mesmos métodos do ICategoriaRepository, para a entidade ModoDisponibilizacao.
public interface IModoDisponibilizacaoRepository
{
    // Todos os modos, por nome (não têm Ativo: são só dois, Listagem e Reserva).
    Task<List<ModoDisponibilizacao>> GetAllAsync();

    // Um modo, ou null se não existir.
    Task<ModoDisponibilizacao?> GetByIdAsync(int id);

    // Existe um modo com este id? (o AlojamentoService usa-o antes de gravar um alojamento)
    Task<bool> ExisteAsync(int id);

    // Já há outro modo com este nome? excetoId: no PUT, ignora o próprio.
    Task<bool> NomeExisteAsync(string nome, int? excetoId = null);

    // Há alojamentos com este modo? (se houver, não se pode apagar: RN30)
    Task<bool> TemAlojamentosAsync(int id);

    // Gravações: devolvem false se a base de dados recusar.
    Task<bool> AddAsync(ModoDisponibilizacao modo);
    Task<bool> UpdateAsync(ModoDisponibilizacao modo);
    Task<bool> DeleteAsync(ModoDisponibilizacao modo);
}
