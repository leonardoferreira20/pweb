using RESTfulAPI.DTOs;
using RESTfulAPI.Erros;
using RESTfulAPI.Mappers;
using RESTfulAPI.Repositories;

namespace RESTfulAPI.Services;

// As leituras dos regimes alimentares e das características. O mesmo desenho do HotelService.
public class RegimeAlimentarService(IRegimeAlimentarRepository regimes) : IRegimeAlimentarService
{
    public async Task<List<RegimeAlimentarDto>> GetAllAsync() =>
        (await regimes.GetAllAsync()).Select(r => r.ToDto()).ToList();

    public async Task<Resultado<RegimeAlimentarDto>> GetByIdAsync(int id)
    {
        var regime = await regimes.GetByIdAsync(id);
        if (regime is null)
            return Resultado<RegimeAlimentarDto>.NaoEncontrado(CodigosErro.RegimeNaoEncontrado, $"O regime alimentar {id} não existe.");

        return Resultado<RegimeAlimentarDto>.Ok(regime.ToDto());
    }
}

public class CaracteristicaService(ICaracteristicaRepository caracteristicas) : ICaracteristicaService
{
    public async Task<List<CaracteristicaDto>> GetAllAsync() =>
        (await caracteristicas.GetAllAsync()).Select(c => c.ToDto()).ToList();

    public async Task<Resultado<CaracteristicaDto>> GetByIdAsync(int id)
    {
        var caracteristica = await caracteristicas.GetByIdAsync(id);
        if (caracteristica is null)
            return Resultado<CaracteristicaDto>.NaoEncontrado(CodigosErro.CaracteristicaNaoEncontrada, $"A característica {id} não existe.");

        return Resultado<CaracteristicaDto>.Ok(caracteristica.ToDto());
    }
}
