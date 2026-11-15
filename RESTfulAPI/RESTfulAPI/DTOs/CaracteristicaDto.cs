namespace RESTfulAPI.DTOs;

// O que sai: a resposta de GET /api/caracteristicas, e cada característica dentro do AlojamentoDto.
/// <summary>Uma característica (comodidade) de um alojamento.</summary>
public class CaracteristicaDto
{
    /// <summary>O id da característica.</summary>
    /// <example>4</example>
    public int Id { get; set; }

    /// <summary>O nome da característica.</summary>
    /// <example>Vista mar</example>
    public string Nome { get; set; } = "";

    /// <summary>Está ativa? As inativas não aparecem na listagem.</summary>
    /// <example>true</example>
    public bool Ativo { get; set; }
}
