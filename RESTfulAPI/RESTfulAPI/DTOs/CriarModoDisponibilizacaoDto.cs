using System.ComponentModel.DataAnnotations;

namespace RESTfulAPI.DTOs;

// O que entra no POST /api/modos-disponibilizacao. Sem Id: quem o dá é a base de dados.
/// <summary>O que se envia no POST /api/modos-disponibilizacao.</summary>
public class CriarModoDisponibilizacaoDto
{
    // Obrigatório, até 100 caracteres. Se faltar no JSON, fica "" e o [Required] recusa.
    /// <summary>O nome: obrigatório, até 100 caracteres, único.</summary>
    /// <example>Pedido de orçamento</example>
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome tem no máximo 100 caracteres.")]
    public string Nome { get; set; } = "";

    // Opcional, até 200 caracteres.
    /// <summary>Uma descrição curta (opcional), até 200 caracteres.</summary>
    /// <example>Reserva só depois de contacto com a unidade</example>
    [StringLength(200, ErrorMessage = "A descrição tem no máximo 200 caracteres.")]
    public string? Descricao { get; set; }
}
