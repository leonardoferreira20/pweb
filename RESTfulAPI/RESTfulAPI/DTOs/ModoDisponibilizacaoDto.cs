namespace RESTfulAPI.DTOs;

// O que sai: a resposta de GET /api/modos-disponibilizacao e GET /api/modos-disponibilizacao/{id}.
// Comparado com a entidade ModoDisponibilizacao: não leva a lista de alojamentos.
/// <summary>Um modo de disponibilização, como sai nos GET (e no 201 do POST).</summary>
public class ModoDisponibilizacaoDto
{
    // O id, para a app o usar no CriarAlojamentoDto (ModoDisponibilizacaoId).
    /// <summary>O id do modo de disponibilização.</summary>
    /// <example>2</example>
    public int Id { get; set; }

    // O nome: Listagem (aparece, mas não se reserva) ou Reserva (RN26).
    /// <summary>O nome do modo.</summary>
    /// <example>Reserva</example>
    public string Nome { get; set; } = "";

    // Mais informação, opcional.
    /// <summary>Uma descrição curta, ou null.</summary>
    /// <example>Pode ser reservado</example>
    public string? Descricao { get; set; }
}
