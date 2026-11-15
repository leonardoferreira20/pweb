namespace RESTfulAPI.DTOs;

// O que sai: a resposta de GET /api/hoteis e GET /api/hoteis/{id}.
// Comparado com a entidade Hotel: a morada vem aninhada (em vez do MoradaId),
// não leva os alojamentos nem as campanhas, e não tem o UrlImagem (o nome do ficheiro).
// Nesta etapa a API só lê as unidades hoteleiras: criam-se e alteram-se na Gestão de Hotéis.
/// <summary>Uma unidade hoteleira (hotel ou aparthotel), com a morada.</summary>
public class HotelDto
{
    /// <summary>O id da unidade hoteleira.</summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>O nome da unidade.</summary>
    /// <example>Hotel Atlântico</example>
    public string Nome { get; set; } = "";

    // "Hotel" ou "Aparthotel" (os valores da classe TipoHotel, na Gestão de Hotéis).
    /// <summary>O tipo: Hotel ou Aparthotel.</summary>
    /// <example>Hotel</example>
    public string Tipo { get; set; } = "";

    /// <summary>A classificação, de 1 a 5 estrelas.</summary>
    /// <example>4</example>
    public int Estrelas { get; set; }

    /// <summary>A descrição da unidade.</summary>
    /// <example>Hotel de praia, em frente ao mar, com piscina exterior.</example>
    public string Descricao { get; set; } = "";

    /// <summary>Está ativa? As inativas, e os seus alojamentos, não aparecem nas listagens.</summary>
    /// <example>true</example>
    public bool Ativo { get; set; }

    // Objeto aninhado: a app recebe a morada já completa, sem outro pedido.
    /// <summary>A morada da unidade.</summary>
    public MoradaDto Morada { get; set; } = new();

    // A imagem em bytes: no JSON vai como texto Base64 (ver o comentário no AlojamentoDto).
    /// <summary>A imagem em Base64.</summary>
    public byte[]? Imagem { get; set; }
}
