using System.ComponentModel.DataAnnotations;

namespace GestaoHoteis.Entities;

// Uma morada. A mesma classe serve as unidades hoteleiras, os utilizadores
// e as reservas (a morada de faturação), cada um com a sua própria linha.
// Cada objeto desta classe corresponde a uma linha da tabela Moradas.
public class Morada
{
    // Chave primária, atribuída pela base de dados.
    public int Id { get; set; }

    // Rua, número e andar: obrigatório, até 100 caracteres.
    // ErrorMessage (Etapa 3): a mensagem que o formulário mostra, em português.
    [StringLength(100, ErrorMessage = "A rua tem no máximo 100 caracteres.")]
    [Required(ErrorMessage = "A rua é obrigatória.")]
    public string? Rua { get; set; }

    // Só aceita o formato 1234-567: quatro dígitos, hífen, três dígitos.
    // [StringLength(8)]: 1234-567 tem oito caracteres, e a coluna fica nvarchar(8).
    [StringLength(8)]
    [Required(ErrorMessage = "O código postal é obrigatório.")]
    [RegularExpression(@"^\d{4}-\d{3}$", ErrorMessage = "O código postal deve estar no formato '1234-567'.")]
    public string? CodigoPostal { get; set; }

    // Localidade: obrigatória, até 100 caracteres.
    [StringLength(100, ErrorMessage = "A localidade tem no máximo 100 caracteres.")]
    [Required(ErrorMessage = "A localidade é obrigatória.")]
    public string? Localidade { get; set; }

    // Distrito: obrigatório, até 100 caracteres.
    [StringLength(100, ErrorMessage = "O distrito tem no máximo 100 caracteres.")]
    [Required(ErrorMessage = "O distrito é obrigatório.")]
    public string? Distrito { get; set; }

    // País: obrigatório, até 100 caracteres. Começa preenchido com Portugal.
    [StringLength(100, ErrorMessage = "O país tem no máximo 100 caracteres.")]
    [Required(ErrorMessage = "O país é obrigatório.")]
    public string? Pais { get; set; } = "Portugal";
}