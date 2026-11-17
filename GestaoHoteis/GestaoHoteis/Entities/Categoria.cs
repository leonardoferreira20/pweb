using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GestaoHoteis.Entities;

// A tipologia de um alojamento (por exemplo, "Quarto Duplo" ou "Suite").
// Cada objeto desta classe corresponde a uma linha da tabela Categorias.
public class Categoria
{
    // Chave primária. Por se chamar Id, o EF Core reconhece-a sozinho,
    // e é a base de dados que atribui o valor (1, 2, 3...).
    public int Id { get; set; }

    // Nome da categoria: obrigatório, até 100 caracteres.
    [StringLength(100, ErrorMessage = "O nome tem no máximo 100 caracteres.")]
    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string? Nome { get; set; }

    // Descrição, opcional, até 200 caracteres.
    [StringLength(200)]
    public string? Descricao { get; set; }

    // Ícone (extra do grupo): o nome de um ficheiro de wwwroot/img/icones/categorias,
    // sem a extensão (por exemplo "suite"). Opcional: sem ícone, mostra-se o genérico.
    [StringLength(50)]
    public string? Icone { get; set; }

    // Ícone carregado (extra do grupo): uma imagem PNG ou JPEG própria, em vez de um dos
    // ícones da lista, como a Imagem dos alojamentos. Quando existe, é esta que se mostra,
    // e Icone fica a null.
    public byte[]? IconeImagem { get; set; }

    // Uma categoria inativa não aparece na aplicação dos clientes.
    public bool Ativo { get; set; } = true;

    // Navegação de coleção: os alojamentos desta categoria. Não cria colunas.
    [JsonIgnore]
    public ICollection<Alojamento>? alojamentos { get; set; }
}