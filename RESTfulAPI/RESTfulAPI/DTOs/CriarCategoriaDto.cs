using System.ComponentModel.DataAnnotations;

namespace RESTfulAPI.DTOs;

// O que entra no POST /api/categorias. Sem Id: quem o dá é a base de dados.
// Só tem os campos que a app pode escolher: o nome, a descrição e se está ativa.
// As validações são feitas pelo [ApiController] antes de o método do controller começar:
// se alguma falhar, a resposta é logo 400, com o erro em "errors".
/// <summary>O que se envia no POST /api/categorias.</summary>
public class CriarCategoriaDto
{
    // Obrigatório. string (e não string?) com "= """: se a app não mandar o nome,
    // fica a string vazia, e o [Required] recusa-a.
    /// <summary>O nome: obrigatório, até 100 caracteres, único.</summary>
    /// <example>Apartamento T2</example>
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome tem no máximo 100 caracteres.")]
    public string Nome { get; set; } = "";

    // Opcional, até 200 caracteres.
    /// <summary>Uma descrição curta (opcional), até 200 caracteres.</summary>
    /// <example>Apartamento com dois quartos e cozinha</example>
    [StringLength(200, ErrorMessage = "A descrição tem no máximo 200 caracteres.")]
    public string? Descricao { get; set; }

    // Se não vier no JSON, fica true: uma categoria nova começa ativa, como na Gestão de Hotéis.
    /// <summary>Está ativa? Se não vier, fica true.</summary>
    /// <example>true</example>
    public bool Ativo { get; set; } = true;
}
