using RESTfulAPI.DTOs;
using RESTfulAPI.Erros;
using RESTfulAPI.Mappers;
using RESTfulAPI.Repositories;

namespace RESTfulAPI.Services;

// As regras de negócio dos modos de disponibilização: cumpre o IModoDisponibilizacaoService.
// O mesmo desenho do CategoriaService. Só precisa do repository dos modos.
public class ModoDisponibilizacaoService(IModoDisponibilizacaoRepository modos) : IModoDisponibilizacaoService
{
    // A lista, convertida em DTOs.
    public async Task<List<ModoDisponibilizacaoDto>> GetAllAsync()
    {
        var lista = await modos.GetAllAsync();
        return lista.Select(m => m.ToDto()).ToList();
    }

    // Um modo, ou o erro NaoEncontrado (o controller responde 404).
    public async Task<Resultado<ModoDisponibilizacaoDto>> GetByIdAsync(int id)
    {
        var modo = await modos.GetByIdAsync(id);
        if (modo is null)
            return Resultado<ModoDisponibilizacaoDto>.NaoEncontrado(CodigosErro.ModoNaoEncontrado, $"O modo de disponibilização {id} não existe.");

        return Resultado<ModoDisponibilizacaoDto>.Ok(modo.ToDto());
    }

    // Criar (POST): nome único, converter, gravar.
    public async Task<Resultado<ModoDisponibilizacaoDto>> CreateAsync(CriarModoDisponibilizacaoDto dto)
    {
        // Regra: nomes únicos (409 se já existir).
        if (await modos.NomeExisteAsync(dto.Nome))
            return Resultado<ModoDisponibilizacaoDto>.Conflito(CodigosErro.ModoNomeRepetido, $"Já existe um modo de disponibilização \"{dto.Nome.Trim()}\".");

        var modo = dto.ToEntity();
        if (!await modos.AddAsync(modo))
            return Resultado<ModoDisponibilizacaoDto>.Conflito(CodigosErro.GravacaoRecusada, "A base de dados recusou o modo de disponibilização.");

        return Resultado<ModoDisponibilizacaoDto>.Ok(modo.ToDto());
    }

    // Alterar (PUT): procurar, nome único (sem contar o próprio), converter, gravar.
    public async Task<Resultado> UpdateAsync(int id, AtualizarModoDisponibilizacaoDto dto)
    {
        var modo = await modos.GetByIdAsync(id);
        if (modo is null)
            return Resultado.NaoEncontrado(CodigosErro.ModoNaoEncontrado, $"O modo de disponibilização {id} não existe.");

        if (await modos.NomeExisteAsync(dto.Nome, excetoId: id))
            return Resultado.Conflito(CodigosErro.ModoNomeRepetido, $"Já existe outro modo de disponibilização \"{dto.Nome.Trim()}\".");

        dto.AplicarEm(modo);
        if (!await modos.UpdateAsync(modo))
            return Resultado.Conflito(CodigosErro.GravacaoRecusada, "A base de dados recusou a alteração.");

        return Resultado.Ok();
    }

    // Apagar (DELETE): procurar, verificar que não está em uso, apagar.
    public async Task<Resultado> DeleteAsync(int id)
    {
        var modo = await modos.GetByIdAsync(id);
        if (modo is null)
            return Resultado.NaoEncontrado(CodigosErro.ModoNaoEncontrado, $"O modo de disponibilização {id} não existe.");

        // Regra RN30: um modo usado por alojamentos não se apaga.
        if (await modos.TemAlojamentosAsync(id))
            return Resultado.Conflito(CodigosErro.ModoEmUso, "O modo de disponibilização está a ser usado por alojamentos.");

        if (!await modos.DeleteAsync(modo))
            return Resultado.Conflito(CodigosErro.GravacaoRecusada, "A base de dados recusou apagar o modo de disponibilização.");

        return Resultado.Ok();
    }
}
