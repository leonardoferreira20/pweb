using RESTfulAPI.DTOs;
using RESTfulAPI.Erros;
using RESTfulAPI.Mappers;
using RESTfulAPI.Repositories;

namespace RESTfulAPI.Services;

// As regras de negócio das categorias. Fica entre o Controller (DTOs) e o Repository (entidades).
// Recebe pelo construtor as interfaces dos dois repositories de que precisa
// (o dos alojamentos serve para listar os alojamentos de uma categoria).
// Cada método segue o mesmo desenho: procurar, verificar as regras, converter (Mapper), gravar, devolver.
public class CategoriaService(ICategoriaRepository categorias, IAlojamentoRepository alojamentos) : ICategoriaService
{
    // A lista: pedir as entidades ao Repository e converter cada uma em DTO.
    public async Task<List<CategoriaDto>> GetAllAsync()
    {
        var lista = await categorias.GetAllAsync();
        // Select aplica o ToDto a cada categoria; ToList junta tudo numa lista.
        return lista.Select(c => c.ToDto()).ToList();
    }

    // Uma categoria. Se não existir, devolve o erro (não lança exceção).
    public async Task<Resultado<CategoriaDto>> GetByIdAsync(int id)
    {
        var categoria = await categorias.GetByIdAsync(id);
        if (categoria is null)
            return Resultado<CategoriaDto>.NaoEncontrado(CodigosErro.CategoriaNaoEncontrada, $"A categoria {id} não existe.");

        return Resultado<CategoriaDto>.Ok(categoria.ToDto());
    }

    // GET /api/categorias/{id}/alojamentos: 404 se a categoria não existir;
    // se existir e não tiver alojamentos, uma lista vazia (não é erro).
    public async Task<Resultado<List<AlojamentoDto>>> GetAlojamentosAsync(int id)
    {
        if (!await categorias.ExisteAsync(id))
            return Resultado<List<AlojamentoDto>>.NaoEncontrado(CodigosErro.CategoriaNaoEncontrada, $"A categoria {id} não existe.");

        // Reaproveita a listagem de alojamentos, com o filtro da categoria.
        var lista = await alojamentos.GetAllAsync(hotelId: null, categoriaId: id, estrelas: null, localidade: null, pesquisa: null);
        return Resultado<List<AlojamentoDto>>.Ok(lista.Select(a => a.ToDto()).ToList());
    }

    // Criar (POST).
    public async Task<Resultado<CategoriaDto>> CreateAsync(CriarCategoriaDto dto)
    {
        // O ícone carregado (extra do grupo), se vier, tem de ser uma imagem: a mesma
        // verificação das imagens dos alojamentos (PNG, JPEG, GIF ou WebP, até 2 MB).
        var erro = Imagens.Validar(dto.IconeImagem);
        if (erro is not null)
            return Resultado<CategoriaDto>.Invalido(erro.Codigo!, erro.Mensagem!);

        // Regra: nomes únicos. A base de dados também o garante (índice único),
        // mas assim a mensagem é clara e não se chega a tentar o INSERT.
        if (await categorias.NomeExisteAsync(dto.Nome))
            return Resultado<CategoriaDto>.Conflito(CodigosErro.CategoriaNomeRepetido, $"Já existe uma categoria \"{dto.Nome.Trim()}\".");

        // Mapper: DTO → entidade nova; Repository: INSERT. Depois disto, categoria.Id tem o id novo.
        var categoria = dto.ToEntity();
        if (!await categorias.AddAsync(categoria))
            return Resultado<CategoriaDto>.Conflito(CodigosErro.GravacaoRecusada, "A base de dados recusou a categoria.");

        // Devolve o DTO da categoria criada: o controller põe-no no body do 201.
        return Resultado<CategoriaDto>.Ok(categoria.ToDto());
    }

    // Alterar (PUT).
    public async Task<Resultado> UpdateAsync(int id, AtualizarCategoriaDto dto)
    {
        // 1. Procurar: tem de existir.
        var categoria = await categorias.GetByIdAsync(id);
        if (categoria is null)
            return Resultado.NaoEncontrado(CodigosErro.CategoriaNaoEncontrada, $"A categoria {id} não existe.");

        // O ícone carregado (extra do grupo), se vier, tem de ser uma imagem.
        var erro = Imagens.Validar(dto.IconeImagem);
        if (erro is not null)
            return erro;

        // 2. Regra: o novo nome não pode ser o de outra categoria.
        if (await categorias.NomeExisteAsync(dto.Nome, excetoId: id))
            return Resultado.Conflito(CodigosErro.CategoriaNomeRepetido, $"Já existe outra categoria \"{dto.Nome.Trim()}\".");

        // 3. Converter e gravar: o Mapper copia os campos, o Repository faz o UPDATE.
        dto.AplicarEm(categoria);
        if (!await categorias.UpdateAsync(categoria))
            return Resultado.Conflito(CodigosErro.GravacaoRecusada, "A base de dados recusou a alteração.");

        return Resultado.Ok();
    }

    // Apagar (DELETE).
    public async Task<Resultado> DeleteAsync(int id)
    {
        var categoria = await categorias.GetByIdAsync(id);
        if (categoria is null)
            return Resultado.NaoEncontrado(CodigosErro.CategoriaNaoEncontrada, $"A categoria {id} não existe.");

        // Regra RN18: uma categoria só se apaga se não tiver alojamentos.
        // A base de dados também o recusa (Restrict), mas assim a mensagem diz porquê.
        if (await categorias.TemAlojamentosAsync(id))
            return Resultado.Conflito(CodigosErro.CategoriaComAlojamentos, "A categoria tem alojamentos. Mude-os de categoria, ou desative a categoria.");

        if (!await categorias.DeleteAsync(categoria))
            return Resultado.Conflito(CodigosErro.GravacaoRecusada, "A base de dados recusou apagar a categoria.");

        return Resultado.Ok();
    }
}
