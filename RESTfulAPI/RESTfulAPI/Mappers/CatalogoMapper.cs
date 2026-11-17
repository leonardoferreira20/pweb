using RESTfulAPI.DTOs;
using RESTfulAPI.Entities;

namespace RESTfulAPI.Mappers;

// Converte as entidades que a API só lê (regimes alimentares e características) nos seus DTOs.
// Só têm ToDto: nesta etapa a API não as cria nem as altera (fazem-se na Gestão de Hotéis).
// Estão no mesmo ficheiro porque são duas linhas cada.
public static class CatalogoMapper
{
    // Entidade → DTO (GET /api/regimes-alimentares, e dentro do AlojamentoDto).
    public static RegimeAlimentarDto ToDto(this RegimeAlimentar r) => new()
    {
        Id = r.Id,
        Nome = r.Nome ?? "",
        Descricao = r.Descricao,
        Percentagem = r.Percentagem,
        Icone = r.Icone,
        IconeImagem = r.IconeImagem,
        Ativo = r.Ativo
    };

    // Entidade → DTO (GET /api/caracteristicas, e dentro do AlojamentoDto).
    public static CaracteristicaDto ToDto(this Caracteristica c) => new()
    {
        Id = c.Id,
        Nome = c.Nome ?? "",
        Icone = c.Icone,
        IconeImagem = c.IconeImagem,
        Ativo = c.Ativo
    };
}
