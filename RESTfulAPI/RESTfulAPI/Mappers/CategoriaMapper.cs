using RESTfulAPI.DTOs;
using RESTfulAPI.Entities;

namespace RESTfulAPI.Mappers;

// Converte entre a entidade Categoria e os seus DTOs. Só copia: sem regras nem base de dados.
// Os métodos são de extensão (o "this" no primeiro parâmetro): escreve-se categoria.ToDto(),
// como se ToDto fosse um método da classe Categoria, sem mexer nela.
// A classe é static porque só tem métodos: não se criam objetos CategoriaMapper.
public static class CategoriaMapper
{
    // Entidade → DTO de saída (GET).
    // "=> new() { ... }" cria e devolve um CategoriaDto com estes valores.
    public static CategoriaDto ToDto(this Categoria c) => new()
    {
        Id = c.Id,
        // "?? """: se o nome for null, usa texto vazio (o DTO não aceita null).
        Nome = c.Nome ?? "",
        Descricao = c.Descricao,
        Ativo = c.Ativo
    };

    // DTO de entrada → entidade nova (POST). O Id fica 0: é a base de dados que o dá.
    // Trim() tira os espaços no início e no fim: " Suite " fica "Suite".
    public static Categoria ToEntity(this CriarCategoriaDto d) => new()
    {
        Nome = d.Nome.Trim(),
        // Texto opcional: só espaços (ou nada) fica null; senão, sem os espaços nas pontas.
        Descricao = string.IsNullOrWhiteSpace(d.Descricao) ? null : d.Descricao.Trim(),
        Ativo = d.Ativo
    };

    // DTO de entrada → entidade que já existe (PUT). Muda os campos que o DTO traz.
    // Não devolve nada (void): altera o objeto c que recebe, e o EF Core repara na mudança.
    public static void AplicarEm(this AtualizarCategoriaDto d, Categoria c)
    {
        c.Nome = d.Nome.Trim();
        c.Descricao = string.IsNullOrWhiteSpace(d.Descricao) ? null : d.Descricao.Trim();
        c.Ativo = d.Ativo;
    }
}
