using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GestaoHoteis.Entities;

// Uma comodidade ou particularidade de um alojamento (por exemplo, "Varanda" ou "Vista mar").
// Cada objeto desta classe corresponde a uma linha da tabela Caracteristicas.
public class Caracteristica
{
    // Chave primária, atribuída pela base de dados.
    public int Id { get; set; }

    // Nome da característica: obrigatório, até 100 caracteres.
    [StringLength(100, ErrorMessage = "O nome tem no máximo 100 caracteres.")]
    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string? Nome { get; set; }

    // Ícone (extra do grupo): o nome de um ficheiro de wwwroot/img/icones/caracteristicas,
    // sem a extensão (por exemplo "wifi"). Opcional: sem ícone, mostra-se o genérico.
    [StringLength(50)]
    public string? Icone { get; set; }

    // Ícone carregado (extra do grupo): uma imagem PNG ou JPEG própria, em vez de um dos
    // ícones da lista, como a Imagem dos alojamentos. Quando existe, é esta que se mostra,
    // e Icone fica a null.
    public byte[]? IconeImagem { get; set; }

    // Uma característica inativa não aparece na aplicação dos clientes.
    public bool Ativo { get; set; } = true;

    // Relação N-M com Alojamento: os alojamentos que têm esta característica.
    // É o outro lado de Alojamento.caracteristicas. Não cria colunas.
    [JsonIgnore]
    public ICollection<Alojamento>? alojamentos { get; set; }
}