using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RESTfulAPI.Entities;

// Um regime alimentar (por exemplo, "Pequeno-almoço" ou "Meia pensão").
// Cada objeto desta classe corresponde a uma linha da tabela RegimesAlimentares.
// Igual à classe da Gestão de Hotéis (Etapa 3), sem o [JsonIgnore]: na API quem vai para JSON são os DTOs.
public class RegimeAlimentar
{
    // Chave primária, atribuída pela base de dados.
    public int Id { get; set; }

    // Nome do regime: obrigatório, até 100 caracteres.
    [StringLength(100, ErrorMessage = "O nome tem no máximo 100 caracteres.")]
    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string? Nome { get; set; }

    // Descrição, opcional, até 200 caracteres.
    [StringLength(200)]
    public string? Descricao { get; set; }

    // Acréscimo sobre o preço por noite, em percentagem (10 = mais 10%).
    // É igual para todos os alojamentos. decimal(5,2): até 999,99.
    // [Range] (Gestão de Hotéis, Etapa 3): um acréscimo não pode ser negativo.
    [Column(TypeName = "decimal(5,2)")]
    [Range(0, 999.99, ErrorMessage = "A percentagem tem de estar entre 0 e 999,99.")]
    public decimal Percentagem { get; set; }

    // Um regime inativo não pode ser escolhido em novas reservas.
    public bool Ativo { get; set; } = true;

    // Relação N-M com Alojamento: os alojamentos que oferecem este regime.
    // É o outro lado de Alojamento.regimes. Não cria colunas.
    public ICollection<Alojamento>? alojamentos { get; set; }
}
