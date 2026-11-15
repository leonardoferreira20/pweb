namespace RESTfulAPI.DTOs;

// O que sai: a resposta de GET /api/categorias e GET /api/categorias/{id}.
// Comparado com a entidade Categoria: não leva a lista de alojamentos
// (se levasse, cada categoria arrastava os seus alojamentos, com as imagens, para o JSON).
// No JSON, os nomes ficam em camelCase: Id → "id", Descricao → "descricao".
/// <summary>Uma categoria (tipologia de alojamento), como sai nos GET (e no 201 do POST).</summary>
public class CategoriaDto
{
    // O id, para a app poder pedir esta categoria, alterá-la ou apagá-la.
    /// <summary>O id da categoria.</summary>
    /// <example>2</example>
    public int Id { get; set; }

    // "= """: começa com texto vazio em vez de null.
    /// <summary>O nome da categoria.</summary>
    /// <example>Quarto Duplo</example>
    public string Nome { get; set; } = "";

    // Opcional: null quando a categoria não tem descrição.
    /// <summary>Uma descrição curta, ou null.</summary>
    /// <example>Quarto para duas pessoas</example>
    public string? Descricao { get; set; }

    // Inativa: a app dos clientes não a mostra (RN17). As listagens da API só trazem as ativas.
    /// <summary>Está ativa? As inativas não aparecem na listagem.</summary>
    /// <example>true</example>
    public bool Ativo { get; set; }
}
