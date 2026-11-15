using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RESTfulAPI.Entities;

// Uma tipologia de quarto ou de apartamento de uma unidade hoteleira
// (por exemplo, "Quarto Duplo Vista Mar" do Hotel Atlântico, com 12 unidades).
// Cada objeto desta classe corresponde a uma linha da tabela Alojamentos.
// Igual à classe da Gestão de Hotéis (Etapa 3), sem o [JsonIgnore]: na API quem vai para JSON são os DTOs.
public class Alojamento
{
    // Chave primária, atribuída pela base de dados.
    public int Id { get; set; }

    // Nome do alojamento: obrigatório, até 100 caracteres.
    [StringLength(100, ErrorMessage = "O nome tem no máximo 100 caracteres.")]
    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string? Nome { get; set; }

    // Descrição: obrigatória, até 500 caracteres.
    [StringLength(500, ErrorMessage = "A descrição tem no máximo 500 caracteres.")]
    [Required(ErrorMessage = "A descrição é obrigatória.")]
    public string? Descricao { get; set; }

    // Número máximo de pessoas em cada unidade (cada quarto ou apartamento).
    // [Range] (Gestão de Hotéis, Etapa 3): pelo menos uma pessoa.
    [Range(1, 50, ErrorMessage = "A capacidade tem de estar entre 1 e 50 pessoas.")]
    public int Capacidade { get; set; } = 2;

    // Quantas unidades desta tipologia existem na unidade hoteleira.
    // [Range] (Gestão de Hotéis, Etapa 3): pelo menos uma unidade.
    [Range(1, 10000, ErrorMessage = "Tem de existir pelo menos uma unidade.")]
    public int NumUnidades { get; set; } = 1;

    // Preço por unidade e por noite. [Column] fixa o tipo da coluna: 10 dígitos, 2 decimais.
    // Para dinheiro usa-se decimal, e não double, para não haver erros de arredondamento.
    // [Range] (Gestão de Hotéis, Etapa 3): o preço tem de ser maior do que zero.
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    [Range(0.01, 99999999.99, ErrorMessage = "O preço por noite tem de ser maior do que zero.")]
    public decimal PrecoNoite { get; set; }

    // Nome do ficheiro da imagem, até 200 caracteres.
    [StringLength(200)]
    public string? UrlImagem { get; set; }

    // A imagem em si, guardada na base de dados como bytes (varbinary).
    public byte[]? Imagem { get; set; }

    // Um alojamento inativo não aparece na aplicação dos clientes.
    public bool Ativo { get; set; } = true;

    // Relação com Hotel (obrigatória): o alojamento pertence a uma única unidade.
    public int HotelId { get; set; }
    // Navegação de referência: a unidade hoteleira do alojamento.
    public Hotel hotel { get; set; } = null!;

    // Relação com Categoria (obrigatória).
    public int CategoriaId { get; set; }
    public Categoria categoria { get; set; } = null!;

    // Relação com ModoDisponibilizacao (obrigatória): Listagem ou Reserva.
    public int ModoDisponibilizacaoId { get; set; }
    public ModoDisponibilizacao modoDisponibilizacao { get; set; } = null!;

    // Relação N-M com RegimeAlimentar: os regimes que o alojamento oferece.
    // Uma coleção de cada lado (esta e RegimeAlimentar.alojamentos) chega: o EF Core
    // cria sozinho a tabela de ligação AlojamentoRegimeAlimentar, com as duas chaves.
    // Começa com uma lista vazia, para se poder logo acrescentar regimes.
    public ICollection<RegimeAlimentar> regimes { get; set; } = new List<RegimeAlimentar>();

    // Relação N-M com Caracteristica: as comodidades do alojamento.
    // Tabela de ligação criada pelo EF Core: AlojamentoCaracteristica.
    public ICollection<Caracteristica> caracteristicas { get; set; } = new List<Caracteristica>();
}
