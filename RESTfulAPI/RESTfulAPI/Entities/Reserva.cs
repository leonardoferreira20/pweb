using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RESTfulAPI.Data;

namespace RESTfulAPI.Entities;

// Os estados possíveis de uma reserva, com nome.
public static class EstadoReserva
{
    public const string Pendente = "Pendente";
    public const string Confirmada = "Confirmada";
    public const string Rejeitada = "Rejeitada";
    public const string Expedida = "Expedida";
}

// Uma reserva de um cliente, num hotel, para um período (entrada e saída).
// Os alojamentos reservados são as linhas da reserva (LinhaReserva).
// Cada objeto desta classe corresponde a uma linha da tabela Reservas.
// IValidatableObject (Gestão de Hotéis, Etapa 3): o método Validate confere as duas datas.
// Igual à classe da Gestão de Hotéis (Etapa 3), sem o [JsonIgnore]: na API quem vai para JSON são os DTOs.
public class Reserva : IValidatableObject
{
    // Chave primária, atribuída pela base de dados.
    public int Id { get; set; }

    // Relação com ApplicationUser (obrigatória): o cliente que fez a reserva.
    // A chave do utilizador é um texto (o Id do Identity), por isso a chave estrangeira é string.
    [Required]
    public string? ClienteId { get; set; }
    public ApplicationUser cliente { get; set; } = null!;

    // Relação com Hotel (obrigatória): todos os alojamentos da reserva são desta unidade.
    public int HotelId { get; set; }
    public Hotel hotel { get; set; } = null!;

    // Relação com Morada (1-1, obrigatória): a morada de faturação, só desta reserva.
    public int MoradaFaturacaoId { get; set; }
    public Morada moradaFaturacao { get; set; } = null!;

    // Quando a reserva foi finalizada (hora UTC).
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    // Datas de entrada e de saída, iguais para todos os alojamentos da reserva.
    public DateTime DataEntrada { get; set; }

    public DateTime DataSaida { get; set; }

    // Estado da reserva (os valores da classe EstadoReserva). Uma reserva nova fica Pendente.
    [StringLength(20)]
    public string Estado { get; set; } = EstadoReserva.Pendente;

    // Total da reserva, fixado no momento em que é feita.
    [Column(TypeName = "decimal(10,2)")]
    public decimal Total { get; set; }

    // Navegação de coleção: as linhas da reserva (um alojamento reservado por linha).
    public ICollection<LinhaReserva> linhas { get; set; } = new List<LinhaReserva>();

    // Navegações de referência dos dois 1-1: o pagamento e a fatura da reserva.
    // Deste lado são opcionais (?): a chave estrangeira está em Pagamento e em Fatura,
    // e uma reserva pendente ou rejeitada não tem fatura.
    public Pagamento? pagamento { get; set; }
    public Fatura? fatura { get; set; }

    // Regra que envolve dois campos (Gestão de Hotéis, Etapa 3): pelo menos uma noite.
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DataSaida <= DataEntrada)
        {
            yield return new ValidationResult(
                "A data de saída tem de ser posterior à data de entrada.",
                [nameof(DataSaida)]);
        }
    }
}
