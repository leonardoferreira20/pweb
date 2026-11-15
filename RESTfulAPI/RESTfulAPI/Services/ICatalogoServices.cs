using RESTfulAPI.DTOs;

namespace RESTfulAPI.Services;

// O que os controllers podem pedir sobre regimes alimentares e características. Só leituras.
public interface IRegimeAlimentarService
{
    // GET /api/regimes-alimentares: os regimes ativos.
    Task<List<RegimeAlimentarDto>> GetAllAsync();

    // GET /api/regimes-alimentares/{id}: o regime, ou NaoEncontrado.
    Task<Resultado<RegimeAlimentarDto>> GetByIdAsync(int id);
}

public interface ICaracteristicaService
{
    // GET /api/caracteristicas: as características ativas.
    Task<List<CaracteristicaDto>> GetAllAsync();

    // GET /api/caracteristicas/{id}: a característica, ou NaoEncontrado.
    Task<Resultado<CaracteristicaDto>> GetByIdAsync(int id);
}
