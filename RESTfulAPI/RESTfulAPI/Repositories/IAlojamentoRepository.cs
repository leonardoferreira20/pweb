using RESTfulAPI.Entities;

namespace RESTfulAPI.Repositories;

// O que se pode pedir à base de dados sobre alojamentos.
// Os alojamentos vêm sempre com a unidade, a categoria, o modo, os regimes e as características
// (o Mapper precisa deles).
public interface IAlojamentoRepository
{
    // A lista de alojamentos ativos de unidades ativas (RN04, RN11), com filtros.
    // Os filtros são opcionais: null quer dizer "sem esse filtro".
    // Vêm do URL: GET /api/alojamentos?hotelId=1&categoriaId=2&estrelas=4&localidade=Porto&pesquisa=suite
    Task<List<Alojamento>> GetAllAsync(int? hotelId, int? categoriaId, int? estrelas, string? localidade, string? pesquisa);

    // Um alojamento, ativo ou não, ou null se não existir.
    Task<Alojamento?> GetByIdAsync(int id);

    // Já há, na mesma unidade, outro alojamento com este nome? excetoId: no PUT, ignora o próprio.
    Task<bool> NomeExisteAsync(int hotelId, string nome, int? excetoId = null);

    // O alojamento aparece em alguma reserva? (se aparecer, não se pode apagar: RN12)
    Task<bool> TemReservasAsync(int id);

    // Gravações: devolvem false se a base de dados recusar.
    Task<bool> AddAsync(Alojamento alojamento);
    Task<bool> UpdateAsync(Alojamento alojamento);
    Task<bool> DeleteAsync(Alojamento alojamento);
}
