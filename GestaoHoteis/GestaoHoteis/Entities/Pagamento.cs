using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GestaoHoteis.Entities;

// Os métodos de pagamento aceites, com nome.
public static class MetodoPagamento
{
    public const string Cartao = "Cartão";
    public const string MBWay = "MB WAY";
    public const string Multibanco = "Multibanco";
}

// Os estados possíveis de um pagamento, com nome.
public static class EstadoPagamento
{
    public const string Pago = "Pago";
    public const string Reembolsado = "Reembolsado";
}

// O pagamento (simulado) de uma reserva. Não guarda nenhum dado do meio de pagamento.
// O valor é o total da reserva.
// Cada objeto desta classe corresponde a uma linha da tabela Pagamentos.
public class Pagamento
{
    // Chave primária, atribuída pela base de dados.
    public int Id { get; set; }

    // Relação com Reserva (1-1, obrigatória): cada reserva tem um único pagamento.
    // Como Reserva também tem uma navegação de referência (pagamento), o EF Core
    // reconhece sozinho o 1-1, e é deste lado que fica a chave estrangeira.
    public int ReservaId { get; set; }
    [JsonIgnore]
    public Reserva reserva { get; set; } = null!;

    // Cartão, MB WAY ou Multibanco (os valores da classe MetodoPagamento).
    [StringLength(20)]
    [Required]
    public string? Metodo { get; set; }

    // Quando o pagamento foi feito (hora UTC).
    public DateTime DataPagamento { get; set; } = DateTime.UtcNow;

    // Pago ou Reembolsado (os valores da classe EstadoPagamento).
    [StringLength(20)]
    public string Estado { get; set; } = EstadoPagamento.Pago;
}