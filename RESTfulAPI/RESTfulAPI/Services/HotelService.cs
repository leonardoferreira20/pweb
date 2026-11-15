using RESTfulAPI.DTOs;
using RESTfulAPI.Erros;
using RESTfulAPI.Mappers;
using RESTfulAPI.Repositories;

namespace RESTfulAPI.Services;

// As leituras das unidades hoteleiras: cumpre o IHotelService.
// Sem regras de gravação: só procurar, converter e devolver.
public class HotelService(IHotelRepository hoteis, IAlojamentoRepository alojamentos) : IHotelService
{
    public async Task<List<HotelDto>> GetAllAsync()
    {
        var lista = await hoteis.GetAllAsync();
        return lista.Select(h => h.ToDto()).ToList();
    }

    public async Task<Resultado<HotelDto>> GetByIdAsync(int id)
    {
        var hotel = await hoteis.GetByIdAsync(id);
        if (hotel is null)
            return Resultado<HotelDto>.NaoEncontrado(CodigosErro.HotelNaoEncontrado, $"A unidade hoteleira {id} não existe.");

        return Resultado<HotelDto>.Ok(hotel.ToDto());
    }

    // Os alojamentos de uma unidade: a listagem de alojamentos, com o filtro da unidade.
    // Se a unidade estiver inativa, a lista vem vazia (os alojamentos de uma unidade inativa não se mostram: RN04).
    public async Task<Resultado<List<AlojamentoDto>>> GetAlojamentosAsync(int id)
    {
        if (!await hoteis.ExisteAsync(id))
            return Resultado<List<AlojamentoDto>>.NaoEncontrado(CodigosErro.HotelNaoEncontrado, $"A unidade hoteleira {id} não existe.");

        var lista = await alojamentos.GetAllAsync(hotelId: id, categoriaId: null, estrelas: null, localidade: null, pesquisa: null);
        return Resultado<List<AlojamentoDto>>.Ok(lista.Select(a => a.ToDto()).ToList());
    }
}
