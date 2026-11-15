using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GestaoHoteis.Entities;

// Uma linha de uma reserva: um alojamento reservado, com a quantidade de unidades,
// o regime alimentar escolhido e o número de hóspedes.
// Cada objeto desta classe corresponde a uma linha da tabela LinhasReserva.
public class LinhaReserva
{
    // Chave primária, atribuída pela base de dados.
    public int Id { get; set; }

    // Relação com Reserva (obrigatória): a reserva a que a linha pertence.
    public int ReservaId { get; set; }
    // [JsonIgnore]: a reserva já mostra as suas linhas; a linha não volta a mostrar a reserva.
    [JsonIgnore]
    public Reserva reserva { get; set; } = null!;

    // Relação com Alojamento (obrigatória): o alojamento reservado.
    public int AlojamentoId { get; set; }
    public Alojamento alojamento { get; set; } = null!;

    // Relação com RegimeAlimentar (obrigatória): um dos regimes que o alojamento oferece.
    public int RegimeAlimentarId { get; set; }
    public RegimeAlimentar regimeAlimentar { get; set; } = null!;

    // Quantas unidades deste alojamento são reservadas.
    // [Range] (Etapa 3): pelo menos uma.
    [Range(1, 10000, ErrorMessage = "A quantidade tem de ser pelo menos 1.")]
    public int Quantidade { get; set; } = 1;

    // Quantas pessoas ficam nestas unidades.
    // [Range] (Etapa 3): pelo menos uma.
    [Range(1, 100000, ErrorMessage = "O número de hóspedes tem de ser pelo menos 1.")]
    public int NumHospedes { get; set; } = 1;

    // Valor da linha, fixado no momento da reserva.
    [Column(TypeName = "decimal(10,2)")]
    public decimal Subtotal { get; set; }
}