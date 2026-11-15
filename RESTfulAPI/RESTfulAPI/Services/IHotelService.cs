using RESTfulAPI.DTOs;

namespace RESTfulAPI.Services;

// O que os controllers podem pedir sobre unidades hoteleiras. Só leituras, nesta etapa.
public interface IHotelService
{
    // GET /api/hoteis: as unidades ativas, com a morada.
    Task<List<HotelDto>> GetAllAsync();

    // GET /api/hoteis/{id}: a unidade, ou NaoEncontrado.
    Task<Resultado<HotelDto>> GetByIdAsync(int id);

    // GET /api/hoteis/{id}/alojamentos: os alojamentos da unidade, ou NaoEncontrado.
    Task<Resultado<List<AlojamentoDto>>> GetAlojamentosAsync(int id);
}
