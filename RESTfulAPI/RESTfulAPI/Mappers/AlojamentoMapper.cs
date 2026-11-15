using RESTfulAPI.DTOs;
using RESTfulAPI.Entities;

namespace RESTfulAPI.Mappers;

// Converte entre a entidade Alojamento e os seus DTOs.
// O mesmo desenho do CategoriaMapper: ToDto (GET), ToEntity (POST), AplicarEm (PUT).
// Não valida nada e não vai à base de dados: isso é com o Service e o Repository.
// As duas relações N-M (regimes e características) também não: para as gravar são precisos
// objetos lidos da base de dados, e quem os vai buscar é o AlojamentoService.
public static class AlojamentoMapper
{
    // Entidade → DTO de saída (GET).
    // Precisa da unidade, da categoria, do modo e das duas coleções carregados (os Include no Repository):
    // sem o Include, a.hotel seria null e esta linha rebentava.
    public static AlojamentoDto ToDto(this Alojamento a) => new()
    {
        Id = a.Id,
        Nome = a.Nome ?? "",
        Descricao = a.Descricao ?? "",
        Capacidade = a.Capacidade,
        NumUnidades = a.NumUnidades,
        PrecoNoite = a.PrecoNoite,
        Ativo = a.Ativo,
        // As referências vão aninhadas, só com o id e o nome.
        Hotel = new ReferenciaDto { Id = a.hotel.Id, Nome = a.hotel.Nome ?? "" },
        Categoria = new ReferenciaDto { Id = a.categoria.Id, Nome = a.categoria.Nome ?? "" },
        ModoDisponibilizacao = new ReferenciaDto { Id = a.modoDisponibilizacao.Id, Nome = a.modoDisponibilizacao.Nome ?? "" },
        // As coleções: o ToDto de cada regime e de cada característica (CatalogoMapper).
        Regimes = a.regimes.OrderBy(r => r.Percentagem).Select(r => r.ToDto()).ToList(),
        Caracteristicas = a.caracteristicas.OrderBy(c => c.Nome).Select(c => c.ToDto()).ToList(),
        // A imagem: os bytes, que no JSON viram Base64 (ver o comentário no AlojamentoDto).
        Imagem = a.Imagem
    };

    // DTO de entrada → entidade nova (POST).
    // Só preenche as chaves estrangeiras (HotelId, CategoriaId, ...): para gravar, chegam.
    // O Service verifica antes que existem, e põe os regimes e as características.
    public static Alojamento ToEntity(this CriarAlojamentoDto d) => new()
    {
        Nome = d.Nome.Trim(),
        Descricao = d.Descricao.Trim(),
        Capacidade = d.Capacidade,
        NumUnidades = d.NumUnidades,
        PrecoNoite = d.PrecoNoite,
        Ativo = d.Ativo,
        HotelId = d.HotelId,
        CategoriaId = d.CategoriaId,
        ModoDisponibilizacaoId = d.ModoDisponibilizacaoId,
        // Os bytes da imagem. O nome do ficheiro (UrlImagem) é o Service que o põe.
        Imagem = d.Imagem
    };

    // DTO de entrada → entidade que já existe (PUT).
    // A entidade veio do Repository com tracking: o EF Core compara-a com a cópia
    // que guardou e o UPDATE só leva as colunas que mudaram.
    public static void AplicarEm(this AtualizarAlojamentoDto d, Alojamento a)
    {
        a.Nome = d.Nome.Trim();
        a.Descricao = d.Descricao.Trim();
        a.Capacidade = d.Capacidade;
        a.NumUnidades = d.NumUnidades;
        a.PrecoNoite = d.PrecoNoite;
        a.Ativo = d.Ativo;
        a.HotelId = d.HotelId;
        a.CategoriaId = d.CategoriaId;
        a.ModoDisponibilizacaoId = d.ModoDisponibilizacaoId;
        a.Imagem = d.Imagem;
    }
}
