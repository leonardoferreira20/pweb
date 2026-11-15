using RESTfulAPI.Entities;

namespace RESTfulAPI.Repositories;

// O que se pode pedir à base de dados sobre regimes alimentares.
// Nesta etapa a API só os lê: criam-se e alteram-se na Gestão de Hotéis.
public interface IRegimeAlimentarRepository
{
    // Os regimes ativos (os que se podem escolher numa reserva: RN35), pela percentagem.
    Task<List<RegimeAlimentar>> GetAllAsync();

    // Um regime, ativo ou não, ou null se não existir.
    Task<RegimeAlimentar?> GetByIdAsync(int id);

    // Os regimes com estes ids, COM tracking: o AlojamentoService põe-nos na coleção
    // regimes de um alojamento, e o EF Core grava as linhas da tabela de ligação.
    // Se algum id não existir, a lista vem com menos elementos.
    Task<List<RegimeAlimentar>> GetByIdsAsync(IEnumerable<int> ids);
}

// O mesmo, para as características.
public interface ICaracteristicaRepository
{
    // As características ativas (RN24), por nome.
    Task<List<Caracteristica>> GetAllAsync();

    // Uma característica, ativa ou não, ou null se não existir.
    Task<Caracteristica?> GetByIdAsync(int id);

    // As características com estes ids, com tracking (para a coleção caracteristicas de um alojamento).
    Task<List<Caracteristica>> GetByIdsAsync(IEnumerable<int> ids);
}
