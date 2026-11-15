using RESTfulAPI.DTOs;
using RESTfulAPI.Entities;
using RESTfulAPI.Erros;
using RESTfulAPI.Mappers;
using RESTfulAPI.Repositories;

namespace RESTfulAPI.Services;

// As regras de negócio dos alojamentos: cumpre o IAlojamentoService.
// Precisa de seis repositories: o dos alojamentos, os das três referências (para verificar que
// a unidade, a categoria e o modo existem) e os dos regimes e das características (as relações N-M).
public class AlojamentoService(
    IAlojamentoRepository alojamentos,
    IHotelRepository hoteis,
    ICategoriaRepository categorias,
    IModoDisponibilizacaoRepository modos,
    IRegimeAlimentarRepository regimes,
    ICaracteristicaRepository caracteristicas) : IAlojamentoService
{
    // A lista, com os filtros que vierem do URL, convertida em DTOs.
    public async Task<List<AlojamentoDto>> GetAllAsync(int? hotelId, int? categoriaId, int? estrelas, string? localidade, string? pesquisa)
    {
        var lista = await alojamentos.GetAllAsync(hotelId, categoriaId, estrelas, localidade, pesquisa);
        return lista.Select(a => a.ToDto()).ToList();
    }

    // Um alojamento, ou NaoEncontrado (404).
    public async Task<Resultado<AlojamentoDto>> GetByIdAsync(int id)
    {
        var alojamento = await alojamentos.GetByIdAsync(id);
        if (alojamento is null)
            return Resultado<AlojamentoDto>.NaoEncontrado(CodigosErro.AlojamentoNaoEncontrado, $"O alojamento {id} não existe.");

        return Resultado<AlojamentoDto>.Ok(alojamento.ToDto());
    }

    // Criar (POST). As validações dos campos já foram feitas pelo [ApiController] (no DTO);
    // aqui ficam as regras que precisam da base de dados.
    public async Task<Resultado<AlojamentoDto>> CreateAsync(CriarAlojamentoDto dto)
    {
        // Regras que o DTO não consegue verificar sozinho: a imagem (PNG, JPEG, GIF ou WebP, até 2 MB),
        // e, na base de dados, se a unidade, a categoria e o modo existem.
        // "??": se a imagem não tiver erro (null), passa a verificar as referências.
        // O "!" diz ao compilador que Codigo e Mensagem não são null (num erro, nunca são).
        var erro = Imagens.Validar(dto.Imagem)
            ?? await VerificarReferenciasAsync(dto.HotelId, dto.CategoriaId, dto.ModoDisponibilizacaoId);
        if (erro is not null)
            return Resultado<AlojamentoDto>.Invalido(erro.Codigo!, erro.Mensagem!);

        // As relações N-M: os regimes e as características escolhidos, lidos da base de dados.
        var (listaRegimes, listaCaracteristicas, erroNM) = await LerRelacoesAsync(dto.RegimesIds, dto.CaracteristicasIds);
        if (erroNM is not null)
            return Resultado<AlojamentoDto>.Invalido(erroNM.Codigo!, erroNM.Mensagem!);

        // Regra: na mesma unidade, os nomes não se repetem (409).
        if (await alojamentos.NomeExisteAsync(dto.HotelId, dto.Nome))
            return Resultado<AlojamentoDto>.Conflito(CodigosErro.AlojamentoNomeRepetido, $"A unidade já tem um alojamento \"{dto.Nome.Trim()}\".");

        var alojamento = dto.ToEntity();                        // Mapper: DTO → entidade (Id = 0)
        alojamento.regimes = listaRegimes;                      // as duas coleções N-M
        alojamento.caracteristicas = listaCaracteristicas;
        alojamento.UrlImagem = Imagens.NomeFicheiro("alojamento", alojamento.Imagem);  // "alojamento.png"
        if (!await alojamentos.AddAsync(alojamento))            // INSERT; o EF preenche alojamento.Id
            return Resultado<AlojamentoDto>.Conflito(CodigosErro.GravacaoRecusada, "A base de dados recusou o alojamento.");

        // Ler outra vez, com a unidade, a categoria e o modo (os Include), para o DTO os ter com o nome.
        var criado = await alojamentos.GetByIdAsync(alojamento.Id);
        return Resultado<AlojamentoDto>.Ok(criado!.ToDto());
    }

    // Alterar (PUT): procurar, verificar, converter, acertar as coleções, gravar.
    public async Task<Resultado> UpdateAsync(int id, AtualizarAlojamentoDto dto)
    {
        var alojamento = await alojamentos.GetByIdAsync(id);
        if (alojamento is null)
            return Resultado.NaoEncontrado(CodigosErro.AlojamentoNaoEncontrado, $"O alojamento {id} não existe.");

        // A imagem tem de servir; a unidade, a categoria e o modo têm de existir.
        var erro = Imagens.Validar(dto.Imagem)
            ?? await VerificarReferenciasAsync(dto.HotelId, dto.CategoriaId, dto.ModoDisponibilizacaoId);
        if (erro is not null)
            return erro;

        var (listaRegimes, listaCaracteristicas, erroNM) = await LerRelacoesAsync(dto.RegimesIds, dto.CaracteristicasIds);
        if (erroNM is not null)
            return erroNM;

        // O novo nome não pode ser o de outro alojamento da mesma unidade.
        if (await alojamentos.NomeExisteAsync(dto.HotelId, dto.Nome, excetoId: id))
            return Resultado.Conflito(CodigosErro.AlojamentoNomeRepetido, $"A unidade já tem outro alojamento \"{dto.Nome.Trim()}\".");

        dto.AplicarEm(alojamento);                              // Mapper: copia os campos simples
        alojamento.UrlImagem = Imagens.NomeFicheiro("alojamento", alojamento.Imagem);

        // As coleções: ficam só os escolhidos. O alojamento veio com as suas coleções (Include),
        // por isso o EF Core sabe o que lá estava: apaga as ligações que saem e insere as que entram.
        alojamento.regimes.Clear();
        foreach (var r in listaRegimes)
            alojamento.regimes.Add(r);
        alojamento.caracteristicas.Clear();
        foreach (var c in listaCaracteristicas)
            alojamento.caracteristicas.Add(c);

        if (!await alojamentos.UpdateAsync(alojamento))         // UPDATE, e as tabelas de ligação
            return Resultado.Conflito(CodigosErro.GravacaoRecusada, "A base de dados recusou a alteração.");

        return Resultado.Ok();
    }

    // Apagar (DELETE).
    public async Task<Resultado> DeleteAsync(int id)
    {
        var alojamento = await alojamentos.GetByIdAsync(id);
        if (alojamento is null)
            return Resultado.NaoEncontrado(CodigosErro.AlojamentoNaoEncontrado, $"O alojamento {id} não existe.");

        // Regra RN12: um alojamento que aparece em reservas não se apaga (desativa-se).
        if (await alojamentos.TemReservasAsync(id))
            return Resultado.Conflito(CodigosErro.AlojamentoComReservas, "O alojamento tem reservas: não pode ser apagado, apenas desativado.");

        if (!await alojamentos.DeleteAsync(alojamento))
            return Resultado.Conflito(CodigosErro.GravacaoRecusada, "A base de dados recusou apagar o alojamento.");

        return Resultado.Ok();
    }

    // A unidade, a categoria e o modo têm de existir.
    // Devolve o erro (um Resultado Invalido), ou null se estiver tudo bem.
    // É private: só este service a usa, no POST e no PUT.
    // Task<Resultado?>: o "?" deixa devolver null (sem erro).
    private async Task<Resultado?> VerificarReferenciasAsync(int hotelId, int categoriaId, int modoId)
    {
        if (!await hoteis.ExisteAsync(hotelId))
            return Resultado.Invalido(CodigosErro.HotelInexistente, $"A unidade hoteleira {hotelId} não existe.");

        if (!await categorias.ExisteAsync(categoriaId))
            return Resultado.Invalido(CodigosErro.CategoriaInexistente, $"A categoria {categoriaId} não existe.");

        if (!await modos.ExisteAsync(modoId))
            return Resultado.Invalido(CodigosErro.ModoInexistente, $"O modo de disponibilização {modoId} não existe.");

        return null;
    }

    // Lê da base de dados os regimes e as características com estes ids.
    // Se a lista lida tiver menos elementos do que os ids pedidos (sem repetidos), há um id que não existe.
    // Devolve três valores de uma vez (um "tuplo"): as duas listas e o erro (ou null).
    private async Task<(List<RegimeAlimentar>, List<Caracteristica>, Resultado?)> LerRelacoesAsync(
        List<int> regimesIds, List<int> caracteristicasIds)
    {
        var listaRegimes = await regimes.GetByIdsAsync(regimesIds);
        if (listaRegimes.Count != regimesIds.Distinct().Count())
            return ([], [], Resultado.Invalido(CodigosErro.RegimeInexistente, "Um dos regimes alimentares não existe."));

        var listaCaracteristicas = await caracteristicas.GetByIdsAsync(caracteristicasIds);
        if (listaCaracteristicas.Count != caracteristicasIds.Distinct().Count())
            return ([], [], Resultado.Invalido(CodigosErro.CaracteristicaInexistente, "Uma das características não existe."));

        return (listaRegimes, listaCaracteristicas, null);
    }
}
