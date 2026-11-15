namespace RESTfulAPI.DTOs;

// O que sai: a resposta de GET /api/alojamentos e GET /api/alojamentos/{id}.
// Comparado com a entidade Alojamento: em vez das três chaves estrangeiras (HotelId, ...),
// a unidade, a categoria e o modo vêm aninhados, já com o nome; os regimes e as características
// (as duas relações N-M) vêm como listas; e não tem o UrlImagem (o nome do ficheiro).
// Não tem validações: é a API que o preenche, a partir da entidade (AlojamentoMapper.ToDto).
/// <summary>Um alojamento, como sai nos GET (e no 201 do POST), com a unidade, a categoria, o modo, os regimes e as características.</summary>
public class AlojamentoDto
{
    // O id, para a app pedir, alterar ou apagar este alojamento.
    /// <summary>O id do alojamento.</summary>
    /// <example>1</example>
    public int Id { get; set; }

    // Os campos que a app mostra, iguais aos da entidade.
    /// <summary>O nome do alojamento.</summary>
    /// <example>Quarto Duplo Vista Mar</example>
    public string Nome { get; set; } = "";
    /// <summary>A descrição do alojamento.</summary>
    /// <example>Quarto duplo com varanda virada ao mar.</example>
    public string Descricao { get; set; } = "";
    /// <summary>Quantas pessoas cabem numa unidade deste alojamento.</summary>
    /// <example>2</example>
    public int Capacidade { get; set; }
    /// <summary>Quantas unidades desta tipologia existem na unidade hoteleira.</summary>
    /// <example>12</example>
    public int NumUnidades { get; set; }
    /// <summary>O preço por noite, em euros, sem regime alimentar.</summary>
    /// <example>95.00</example>
    public decimal PrecoNoite { get; set; }
    /// <summary>Está ativo? Os inativos não aparecem na listagem.</summary>
    /// <example>true</example>
    public bool Ativo { get; set; }

    // Objetos aninhados: { "id": 1, "nome": "Hotel Atlântico" }.
    // Em vez do HotelId da entidade: a app recebe já o nome, sem outro pedido.
    // "= new()": começa com um ReferenciaDto vazio, para nunca ser null.
    /// <summary>A unidade hoteleira: o id e o nome.</summary>
    public ReferenciaDto Hotel { get; set; } = new();
    /// <summary>A categoria: o id e o nome.</summary>
    public ReferenciaDto Categoria { get; set; } = new();
    // Listagem: aparece, mas não se reserva; Reserva: pode ser reservado (RN27 e RN28).
    /// <summary>O modo de disponibilização (Listagem ou Reserva): o id e o nome.</summary>
    public ReferenciaDto ModoDisponibilizacao { get; set; } = new();

    // As relações N-M: listas de DTOs. "= []" começa com uma lista vazia.
    // O preço com cada regime calcula-se na app: precoNoite × (1 + percentagem / 100) (RN45).
    /// <summary>Os regimes alimentares que o alojamento oferece (pelo menos um).</summary>
    public List<RegimeAlimentarDto> Regimes { get; set; } = [];
    /// <summary>As características do alojamento.</summary>
    public List<CaracteristicaDto> Caracteristicas { get; set; } = [];

    // A imagem em bytes. No JSON vai como texto Base64 ("iVBORw0KGgo..."), ou null se não tiver.
    // É o mesmo campo que a app manda no POST e no PUT: a imagem entra e sai da mesma maneira.
    // ATENÇÃO: não é a forma mais correta. A imagem vai em TODOS os alojamentos de cada listagem,
    // e o Base64 ocupa mais um terço do que os bytes: as respostas ficam pesadas.
    // Fica assim porque as apps (Blazor, MAUI) mostram listagens com a imagem de cada alojamento,
    // e com os bytes no JSON basta um pedido. Numa API a sério, a imagem teria um endpoint
    // seu (/api/alojamentos/{id}/imagem), e o JSON levaria só o endereço.
    /// <summary>A imagem em Base64.</summary>
    public byte[]? Imagem { get; set; }
}
