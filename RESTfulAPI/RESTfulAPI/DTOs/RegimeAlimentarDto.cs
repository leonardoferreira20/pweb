namespace RESTfulAPI.DTOs;

// O que sai: a resposta de GET /api/regimes-alimentares, e cada regime dentro do AlojamentoDto.
// A percentagem é o acréscimo sobre o preço por noite (RN34). O preço com o regime
// calcula-se na app: precoNoite × (1 + percentagem / 100) (RN45).
/// <summary>Um regime alimentar, com o acréscimo sobre o preço por noite.</summary>
public class RegimeAlimentarDto
{
    /// <summary>O id do regime.</summary>
    /// <example>2</example>
    public int Id { get; set; }

    /// <summary>O nome do regime.</summary>
    /// <example>Pequeno-almoço</example>
    public string Nome { get; set; } = "";

    /// <summary>Uma descrição curta, ou null.</summary>
    /// <example>Alojamento e pequeno-almoço</example>
    public string? Descricao { get; set; }

    // decimal: no JSON é um número com ponto (10.00).
    /// <summary>O acréscimo sobre o preço por noite, em percentagem (0 = sem acréscimo).</summary>
    /// <example>10</example>
    public decimal Percentagem { get; set; }

    // Um regime inativo não pode ser escolhido em novas reservas (RN35).
    /// <summary>Está ativo? Um regime inativo não pode ser escolhido em novas reservas.</summary>
    /// <example>true</example>
    public bool Ativo { get; set; }
}
