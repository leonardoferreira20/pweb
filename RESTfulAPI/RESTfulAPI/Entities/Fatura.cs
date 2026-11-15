using System.ComponentModel.DataAnnotations;

namespace RESTfulAPI.Entities;

// A fatura de uma reserva, emitida quando a reserva é confirmada.
// As linhas e o total são os da reserva; a morada é a de faturação da reserva.
// Cada objeto desta classe corresponde a uma linha da tabela Faturas.
// Igual à classe da Gestão de Hotéis (Etapa 3), sem o [JsonIgnore]: na API quem vai para JSON são os DTOs.
public class Fatura
{
    // Chave primária, atribuída pela base de dados.
    public int Id { get; set; }

    // Relação com Reserva (1-1, obrigatória): cada reserva confirmada tem uma fatura.
    public int ReservaId { get; set; }
    public Reserva reserva { get; set; } = null!;

    // Número da fatura, sequencial.
    public int Numero { get; set; }

    // Quando a fatura foi emitida (hora UTC).
    public DateTime DataEmissao { get; set; } = DateTime.UtcNow;

    // Nome e NIF do cliente no momento da emissão: copiados do utilizador,
    // para que alterações posteriores ao perfil não mudem a fatura.
    [StringLength(200)]
    [Required]
    public string? NomeCliente { get; set; }

    public long NIFCliente { get; set; }
}
