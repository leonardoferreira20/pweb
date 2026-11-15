using System.ComponentModel.DataAnnotations;

namespace RESTfulAPI.Entities;

// Uma comodidade ou particularidade de um alojamento (por exemplo, "Varanda" ou "Vista mar").
// Cada objeto desta classe corresponde a uma linha da tabela Caracteristicas.
// Igual à classe da Gestão de Hotéis (Etapa 3), sem o [JsonIgnore]: na API quem vai para JSON são os DTOs.
public class Caracteristica
{
    // Chave primária, atribuída pela base de dados.
    public int Id { get; set; }

    // Nome da característica: obrigatório, até 100 caracteres.
    [StringLength(100, ErrorMessage = "O nome tem no máximo 100 caracteres.")]
    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string? Nome { get; set; }

    // Uma característica inativa não aparece na aplicação dos clientes.
    public bool Ativo { get; set; } = true;

    // Relação N-M com Alojamento: os alojamentos que têm esta característica.
    // É o outro lado de Alojamento.caracteristicas. Não cria colunas.
    public ICollection<Alojamento>? alojamentos { get; set; }
}
