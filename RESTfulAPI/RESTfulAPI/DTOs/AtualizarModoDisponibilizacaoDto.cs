using System.ComponentModel.DataAnnotations;

namespace RESTfulAPI.DTOs;

// O que entra no PUT /api/modos-disponibilizacao/{id}. Sem Id: o id vem só no URL.
/// <summary>O que se envia no PUT /api/modos-disponibilizacao/{id}: o modo inteiro (o id vai no URL).</summary>
public class AtualizarModoDisponibilizacaoDto
{
    // Obrigatório, até 100 caracteres. Se faltar no JSON, fica "" e o [Required] recusa.
    /// <summary>O nome: obrigatório, até 100 caracteres, único.</summary>
    /// <example>Reserva</example>
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome tem no máximo 100 caracteres.")]
    public string Nome { get; set; } = "";

    // Opcional, até 200 caracteres.
    /// <summary>Uma descrição curta (opcional), até 200 caracteres.</summary>
    /// <example>Pode ser reservado</example>
    [StringLength(200, ErrorMessage = "A descrição tem no máximo 200 caracteres.")]
    public string? Descricao { get; set; }
}
