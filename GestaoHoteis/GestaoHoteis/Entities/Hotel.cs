using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GestaoHoteis.Entities;

// Os tipos de unidade hoteleira, com nome (como o EstadoUtilizador, no ApplicationUser).
// Assim o texto "Aparthotel" fica escrito num só sítio: um engano no nome dá erro de compilação.
public static class TipoHotel
{
    public const string Hotel = "Hotel";
    public const string Aparthotel = "Aparthotel";
}

// Uma unidade hoteleira da empresa: um hotel ou um aparthotel.
// Cada objeto desta classe corresponde a uma linha da tabela Hoteis.
public class Hotel
{
    // Chave primária, atribuída pela base de dados.
    public int Id { get; set; }

    // Nome da unidade: obrigatório, até 100 caracteres.
    [StringLength(100, ErrorMessage = "O nome tem no máximo 100 caracteres.")]
    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string? Nome { get; set; }

    // Hotel ou Aparthotel (os valores da classe TipoHotel, acima).
    // Uma unidade nova começa como Hotel.
    [StringLength(20)]
    public string Tipo { get; set; } = TipoHotel.Hotel;

    // Classificação por estrelas.
    // [Range] (Etapa 3): de 1 a 5.
    [Range(1, 5, ErrorMessage = "A classificação tem de estar entre 1 e 5 estrelas.")]
    public int Estrelas { get; set; } = 3;

    // Descrição da unidade: obrigatória, até 500 caracteres.
    [StringLength(500, ErrorMessage = "A descrição tem no máximo 500 caracteres.")]
    [Required(ErrorMessage = "A descrição é obrigatória.")]
    public string? Descricao { get; set; }

    // Nome do ficheiro da imagem, até 200 caracteres.
    [StringLength(200)]
    public string? UrlImagem { get; set; }

    // A imagem em si, guardada na base de dados como bytes (varbinary).
    public byte[]? Imagem { get; set; }

    // Uma unidade inativa deixa de aparecer na aplicação dos clientes,
    // e os seus alojamentos também. Uma unidade nova começa ativa.
    public bool Ativo { get; set; } = true;

    // Relação com Morada (1-1, obrigatória): cada unidade tem exatamente uma morada.
    // A chave estrangeira fica deste lado. O "1-1" é dito no ApplicationDbContext (alínea 3.c).
    public int MoradaId { get; set; }
    // Navegação de referência: a morada da unidade.
    // = null!: o valor é preenchido pelo EF Core ao ler da base de dados;
    // diz ao compilador para não avisar que a propriedade começa vazia (aviso CS8618).
    public Morada morada { get; set; } = null!;

    // Navegações de coleção: os alojamentos e as campanhas desta unidade.
    // Não criam nenhuma coluna nesta tabela.
    // [JsonIgnore]: ficam de fora quando o objeto é convertido em JSON (evita ciclos na API).
    [JsonIgnore]
    public ICollection<Alojamento>? alojamentos { get; set; }

    [JsonIgnore]
    public ICollection<Campanha>? campanhas { get; set; }
}