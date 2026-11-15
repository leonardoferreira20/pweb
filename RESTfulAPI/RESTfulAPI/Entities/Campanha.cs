using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RESTfulAPI.Entities;

// Uma campanha de uma unidade hoteleira: um desconto ou um aumento de preço
// (época alta), aplicado a todos os alojamentos da unidade durante um período.
// Cada objeto desta classe corresponde a uma linha da tabela Campanhas.
// IValidatableObject (Gestão de Hotéis, Etapa 3): a classe tem um método Validate com as regras
// que envolvem mais de um campo. O formulário chama-o ao validar.
// Igual à classe da Gestão de Hotéis (Etapa 3), sem o [JsonIgnore]: na API quem vai para JSON são os DTOs.
public class Campanha : IValidatableObject
{
    // Chave primária, atribuída pela base de dados.
    public int Id { get; set; }

    // Nome da campanha: obrigatório, até 100 caracteres.
    [StringLength(100, ErrorMessage = "O nome tem no máximo 100 caracteres.")]
    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string? Nome { get; set; }

    // Descrição, opcional, até 200 caracteres.
    [StringLength(200)]
    public string? Descricao { get; set; }

    // Ajuste ao preço por noite, em percentagem:
    // positivo aumenta o preço (15 = mais 15%), negativo é um desconto (-20 = menos 20%).
    // [Range] (Gestão de Hotéis, Etapa 3): um desconto tem de ser inferior a 100%.
    [Column(TypeName = "decimal(5,2)")]
    [Range(-99.99, 999.99, ErrorMessage = "A percentagem tem de estar entre -99,99 e 999,99.")]
    public decimal Percentagem { get; set; }

    // Primeiro e último dia da campanha. Começam preenchidos com o dia de hoje.
    public DateTime DataInicio { get; set; } = DateTime.Today;

    public DateTime DataFim { get; set; } = DateTime.Today;

    // Uma campanha inativa não é aplicada.
    public bool Ativo { get; set; } = true;

    // Relação com Hotel (obrigatória): a campanha é de uma unidade.
    public int HotelId { get; set; }
    public Hotel hotel { get; set; } = null!;

    // Regras que envolvem mais de um campo (Gestão de Hotéis, Etapa 3).
    // Corre depois das regras de cada campo ([Required], [Range], ...), se essas passarem.
    // Cada yield return é um erro: o texto e o campo onde o formulário o mostra.
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Uma campanha de 0% não altera preço nenhum.
        if (Percentagem == 0)
        {
            yield return new ValidationResult(
                "A percentagem não pode ser zero: positiva aumenta o preço, negativa é um desconto.",
                [nameof(Percentagem)]);
        }

        // O período tem de ter pelo menos um dia.
        if (DataFim < DataInicio)
        {
            yield return new ValidationResult(
                "A data de fim não pode ser anterior à data de início.",
                [nameof(DataFim)]);
        }
    }
}
