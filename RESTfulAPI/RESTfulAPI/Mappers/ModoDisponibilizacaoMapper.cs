using RESTfulAPI.DTOs;
using RESTfulAPI.Entities;

namespace RESTfulAPI.Mappers;

// Converte entre a entidade ModoDisponibilizacao e os seus DTOs.
// O mesmo desenho do CategoriaMapper: ToDto (GET), ToEntity (POST), AplicarEm (PUT).
public static class ModoDisponibilizacaoMapper
{
    // Entidade → DTO de saída (GET). A lista de alojamentos fica de fora.
    public static ModoDisponibilizacaoDto ToDto(this ModoDisponibilizacao m) => new()
    {
        Id = m.Id,
        Nome = m.Nome ?? "",
        Descricao = m.Descricao
    };

    // DTO de entrada → entidade nova (POST). O Id fica 0: é a base de dados que o dá.
    public static ModoDisponibilizacao ToEntity(this CriarModoDisponibilizacaoDto d) => new()
    {
        Nome = d.Nome.Trim(),
        Descricao = string.IsNullOrWhiteSpace(d.Descricao) ? null : d.Descricao.Trim()
    };

    // DTO de entrada → entidade que já existe (PUT): copia os novos valores para m.
    public static void AplicarEm(this AtualizarModoDisponibilizacaoDto d, ModoDisponibilizacao m)
    {
        m.Nome = d.Nome.Trim();
        m.Descricao = string.IsNullOrWhiteSpace(d.Descricao) ? null : d.Descricao.Trim();
    }
}
