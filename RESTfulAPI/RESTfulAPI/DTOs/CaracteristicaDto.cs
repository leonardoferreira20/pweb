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

    // Extra do grupo: o nome do ícone (o ficheiro, sem a extensão), ou null.
    // A app tem os seus ficheiros de ícones e mostra o genérico quando é null.
    /// <summary>O nome do ícone (por exemplo "vista-mar"), ou null: a app mostra o genérico.</summary>
    /// <example>vista-mar</example>
    public string? Icone { get; set; }

    // Extra do grupo: o ícone carregado, em Base64, ou null. Quando existe, é este que a app
    // mostra (e "icone" vem a null). Vai no JSON como a imagem dos alojamentos.
    /// <summary>O ícone carregado (PNG ou JPEG) em Base64, ou null: nesse caso usa-se "icone".</summary>
    public byte[]? IconeImagem { get; set; }

    /// <summary>Está ativa? As inativas não aparecem na listagem.</summary>
    /// <example>true</example>
    public bool Ativo { get; set; }
}
