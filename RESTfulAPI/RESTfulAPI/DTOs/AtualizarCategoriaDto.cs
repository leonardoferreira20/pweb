using System.ComponentModel.DataAnnotations;

namespace RESTfulAPI.DTOs;

// O que entra no PUT /api/categorias/{id}. Sem Id: o id vem só no URL.
// Assim não há dúvida sobre qual categoria se altera (se viesse no body, podia ser diferente do URL).
// PUT substitui tudo: a app manda todos os campos, mesmo os que não mudaram.
// Hoje tem os mesmos campos do CriarCategoriaDto, mas é outra classe: se um dia o PUT
// tiver regras diferentes do POST, muda-se só esta.
/// <summary>O que se envia no PUT /api/categorias/{id}: a categoria inteira (o id vai no URL).</summary>
public class AtualizarCategoriaDto
{
    // Obrigatório. string (e não string?) com "= """: se a app não mandar o nome,
    // fica a string vazia, e o [Required] recusa-a.
    /// <summary>O nome: obrigatório, até 100 caracteres, único.</summary>
    /// <example>Quarto Duplo</example>
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome tem no máximo 100 caracteres.")]
    public string Nome { get; set; } = "";

    // Opcional, até 200 caracteres.
    /// <summary>Uma descrição curta (opcional), até 200 caracteres.</summary>
    /// <example>Quarto para duas pessoas</example>
    [StringLength(200, ErrorMessage = "A descrição tem no máximo 200 caracteres.")]
    public string? Descricao { get; set; }

    // Extra do grupo: opcional, até 50 caracteres; só letras minúsculas, algarismos e hífenes
    // (é o nome de um ficheiro). Um nome que a app não conheça mostra o ícone genérico.
    /// <summary>O nome do ícone (opcional), por exemplo "apartamento". Sem ícone, a app mostra o genérico.</summary>
    /// <example>apartamento</example>
    [StringLength(50, ErrorMessage = "O ícone tem no máximo 50 caracteres.")]
    [RegularExpression("^[a-z0-9-]+$", ErrorMessage = "O ícone só pode ter letras minúsculas, algarismos e hífenes.")]
    public string? Icone { get; set; }

    // Extra do grupo: em vez de um ícone da lista, um ícone próprio, em Base64 (opcional).
    // Se vier, o "icone" é ignorado. Que é PNG, JPEG, GIF ou WebP, até 2 MB, verifica-o o Service.
    /// <summary>Um ícone próprio (PNG, JPEG, GIF ou WebP, até 2 MB) em Base64, opcional. Se vier, o "icone" é ignorado.</summary>
    public byte[]? IconeImagem { get; set; }

    // Se não vier no JSON, fica true: uma categoria nova começa ativa, como na Gestão de Hotéis.
    /// <summary>Está ativa? Se não vier, fica true.</summary>
    /// <example>true</example>
    public bool Ativo { get; set; } = true;
}
