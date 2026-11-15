using RESTfulAPI.Entities;

namespace RESTfulAPI.Repositories;

// O que se pode pedir à base de dados sobre unidades hoteleiras.
// Nesta etapa a API só as lê: não há gravações.
public interface IHotelRepository
{
    // As unidades ativas (RN04), por nome, com a morada.
    Task<List<Hotel>> GetAllAsync();

    // Uma unidade, ativa ou não, com a morada; ou null se não existir.
    Task<Hotel?> GetByIdAsync(int id);

    // Existe uma unidade com este id? (o AlojamentoService usa-o antes de gravar um alojamento)
    Task<bool> ExisteAsync(int id);
}
