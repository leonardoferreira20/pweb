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

    // Extra do grupo: o nome do ícone (o ficheiro, sem a extensão), ou null.
    // A app tem os seus ficheiros de ícones e mostra o genérico quando é null.
    /// <summary>O nome do ícone (por exemplo "quarto-duplo"), ou null: a app mostra o genérico.</summary>
    /// <example>quarto-duplo</example>
    public string? Icone { get; set; }

    // Extra do grupo: o ícone carregado, em Base64, ou null. Quando existe, é este que a app
    // mostra (e "icone" vem a null). Vai no JSON como a imagem dos alojamentos.
    /// <summary>O ícone carregado (PNG ou JPEG) em Base64, ou null: nesse caso usa-se "icone".</summary>
    public byte[]? IconeImagem { get; set; }

    // Inativa: a app dos clientes não a mostra (RN17). As listagens da API só trazem as ativas.
    /// <summary>Está ativa? As inativas não aparecem na listagem.</summary>
    /// <example>true</example>
    public bool Ativo { get; set; }
}
