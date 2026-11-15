using System.ComponentModel.DataAnnotations;

namespace RESTfulAPI.DTOs;

// O que entra no PUT /api/alojamentos/{id}: o alojamento inteiro. O id vem só no URL.
// PUT substitui tudo: a app manda todos os campos, mesmo os que não mudaram,
// incluindo as duas listas (os regimes e as características que o alojamento passa a ter)
// e a imagem. As regras são as mesmas do CriarAlojamentoDto.
/// <summary>O que se envia no PUT /api/alojamentos/{id}: o alojamento inteiro (o id vai no URL).</summary>
public class AtualizarAlojamentoDto : IValidatableObject
{
    // Obrigatório, até 100 caracteres. Se faltar no JSON, fica "" e o [Required] recusa.
    /// <summary>O nome: obrigatório, até 100 caracteres, único dentro da unidade hoteleira.</summary>
    /// <example>Quarto Duplo Vista Mar</example>
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome tem no máximo 100 caracteres.")]
    public string Nome { get; set; } = "";

    // Obrigatória, até 500 caracteres.
    /// <summary>A descrição: obrigatória, até 500 caracteres.</summary>
    /// <example>Quarto duplo com varanda virada ao mar.</example>
    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(500, ErrorMessage = "A descrição tem no máximo 500 caracteres.")]
    public string Descricao { get; set; } = "";

    // Os mesmos limites da entidade (Gestão de Hotéis, Etapa 3).
    /// <summary>Quantas pessoas cabem numa unidade: de 1 a 50.</summary>
    /// <example>2</example>
    [Range(1, 50, ErrorMessage = "A capacidade tem de estar entre 1 e 50 pessoas.")]
    public int Capacidade { get; set; }

    /// <summary>Quantas unidades desta tipologia existem: de 1 a 10000.</summary>
    /// <example>12</example>
    [Range(1, 10000, ErrorMessage = "Tem de existir pelo menos uma unidade.")]
    public int NumUnidades { get; set; }

    // Maior do que zero. Se não vier no JSON, fica 0 e o [Range] recusa.
    /// <summary>O preço por noite, maior do que zero, com duas casas decimais no máximo (com ponto: 95.50).</summary>
    /// <example>99.00</example>
    [Range(0.01, 99999999.99, ErrorMessage = "O preço por noite tem de ser maior do que zero.")]
    public decimal PrecoNoite { get; set; }

    // Se não vier no JSON, fica true: um alojamento novo começa ativo.
    /// <summary>Está ativo? Se não vier, fica true.</summary>
    /// <example>true</example>
    public bool Ativo { get; set; } = true;

    // As três chaves estrangeiras são obrigatórias: 0 quer dizer que não vieram no JSON.
    // Um int que falta fica 0, e 0 não é um id válido: daí o Range a começar em 1.
    // Se o id não existir na base de dados, quem o descobre é o AlojamentoService (400 ..._INEXISTENTE).
    /// <summary>O id da unidade hoteleira: obrigatório, e tem de existir.</summary>
    /// <example>1</example>
    [Range(1, int.MaxValue, ErrorMessage = "Escolha uma unidade hoteleira.")]
    public int HotelId { get; set; }

    /// <summary>O id da categoria: obrigatório, e tem de existir.</summary>
    /// <example>2</example>
    [Range(1, int.MaxValue, ErrorMessage = "Escolha uma categoria.")]
    public int CategoriaId { get; set; }

    /// <summary>O id do modo de disponibilização: obrigatório, e tem de existir.</summary>
    /// <example>2</example>
    [Range(1, int.MaxValue, ErrorMessage = "Escolha um modo de disponibilização.")]
    public int ModoDisponibilizacaoId { get; set; }

    // Relação N-M: os ids dos regimes que o alojamento oferece. Pelo menos um (RN32).
    // [MinLength(1)]: numa lista, conta os elementos. "= []": se não vier, fica vazia (e o MinLength recusa).
    /// <summary>Os ids dos regimes alimentares oferecidos: pelo menos um, e todos têm de existir.</summary>
    /// <example>[1, 2, 3]</example>
    [MinLength(1, ErrorMessage = "Escolha pelo menos um regime alimentar.")]
    public List<int> RegimesIds { get; set; } = [];

    // Relação N-M: os ids das características. Pode ficar vazia.
    /// <summary>Os ids das características (pode ser vazia): todos têm de existir.</summary>
    /// <example>[1, 2, 3]</example>
    public List<int> CaracteristicasIds { get; set; } = [];

    // A imagem é obrigatória (RN10). No JSON vem em Base64: "imagem": "iVBORw0KGgo...".
    // Quem converte o ficheiro em Base64 é a app (ou, para testar no Swagger, um site: ver o guia).
    // O ASP.NET Core converte o texto Base64 em bytes; se não for Base64 válido, responde 400.
    // Que é uma imagem PNG, JPEG, GIF ou WebP, até 2 MB, verifica-o o Service (Imagens.Validar).
    /// <summary>A imagem (PNG, JPEG, GIF ou WebP, até 2 MB) em Base64, sem "data:image/...;base64," à frente. Obrigatória. O exemplo é uma bola laranja de 16 x 16 píxeis.</summary>
    /// <example>iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAVElEQVR42mNgoBAw4pL40KPxH5kvUHKDkSgD0DWiA3SDmEjRjE0NEymasallojQQmUi1Hd0V1HHBwBuAK5EQkx6o5wVSXIGslomY9I5PDcWZiWIAABVAJBrTNt3kAAAAAElFTkSuQmCC</example>
    [Required(ErrorMessage = "A imagem é obrigatória.")]
    public byte[]? Imagem { get; set; }

    // A regra que um atributo não consegue escrever: as casas decimais.
    // O [ApiController] só a chama se as regras de cada campo passarem todas;
    // por isso este erro aparece sozinho, depois de corrigidos os outros. Se falhar, responde 400.
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Na base de dados, o preço tem duas casas decimais: 95.999 seria arredondado
        // para 96.00 sem ninguém dar por isso. Mais vale recusar e dizer porquê.
        // "precoNoite": o nome do campo no JSON, para a app pôr o erro junto ao campo certo.
        if (decimal.Round(PrecoNoite, 2) != PrecoNoite)
            yield return new ValidationResult("O preço por noite tem no máximo duas casas decimais.", ["precoNoite"]);
    }
}
