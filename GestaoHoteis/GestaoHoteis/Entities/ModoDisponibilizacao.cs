using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GestaoHoteis.Entities;

// A forma como um alojamento é disponibilizado: só em Listagem (aparece mas não se reserva)
// ou para Reserva. Cada objeto desta classe corresponde a uma linha da tabela ModosDisponibilizacao.
public class ModoDisponibilizacao
{
    // Chave primária, atribuída pela base de dados.
    public int Id { get; set; }

    // Nome do modo: obrigatório, até 100 caracteres.
    [StringLength(100, ErrorMessage = "O nome tem no máximo 100 caracteres.")]
    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string? Nome { get; set; }

    // Descrição, opcional, até 200 caracteres.
    [StringLength(200)]
    public string? Descricao { get; set; }

    // Navegação de coleção: os alojamentos com este modo. Não cria colunas.
    [JsonIgnore]
    public ICollection<Alojamento>? alojamentos { get; set; }
}