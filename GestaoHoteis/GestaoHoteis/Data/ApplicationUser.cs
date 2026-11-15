using GestaoHoteis.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GestaoHoteis.Data;

// Os valores possíveis do estado de um utilizador, com nome.
// A classe é static: não se criam objetos dela, usa-se diretamente, como em
// EstadoUtilizador.Pendente. Assim o texto "Pendente" fica escrito num só sítio:
// um engano no nome dá erro de compilação, em vez de um erro escondido nos dados.
public static class EstadoUtilizador
{
    public const string Pendente = "Pendente";
    public const string Ativo = "Ativo";
    public const string Inativo = "Inativo";
}

// O utilizador da aplicação. Herda de IdentityUser, que já traz o Id, o email,
// o hash da palavra-passe e os restantes campos de autenticação.
// Cada propriedade aqui escrita passa a ser mais uma coluna da tabela AspNetUsers.
public class ApplicationUser : IdentityUser
{
    // Nome próprio, até 100 caracteres.
    [StringLength(100)]
    public string? Nome { get; set; }

    // Apelido, até 100 caracteres.
    [StringLength(100)]
    public string? Apelido { get; set; }

    // Relação com Morada (1-1, opcional): a morada do perfil.
    // int? porque administradores e funcionários não são obrigados a ter morada.
    public int? MoradaId { get; set; }
    // Navegação de referência: o objeto Morada deste utilizador.
    public Morada? morada { get; set; }

    // Só aceita 9 dígitos, o primeiro dos quais 9.
    [RegularExpression(@"^9\d{8}$", ErrorMessage = "O número de telemóvel deve ter 9 dígitos e começar com '9'.")]
    public string? Telemovel { get; set; }

    // Validado pelo atributo validarNIF, definido no fim deste ficheiro.
    [validarNIF(ErrorMessage = "O NIF é inválido.")]
    public long? NIF { get; set; }

    // Matrícula do veículo, opcional. Formato português: AA-00-AA (três pares, com hífenes).
    [StringLength(8)]
    [RegularExpression(@"^[A-Z0-9]{2}-[A-Z0-9]{2}-[A-Z0-9]{2}$", ErrorMessage = "A matrícula deve estar no formato 'AA-00-AA'.")]
    public string? MatriculaVeiculo { get; set; }

    // Fotografia do utilizador, guardada na base de dados como bytes.
    public byte[]? Fotografia { get; set; }

    // Estado da conta. Um utilizador novo começa como Pendente.
    [StringLength(20)]
    public string Estado { get; set; } = EstadoUtilizador.Pendente;

    // Data de registo, preenchida quando o objeto é criado (hora UTC).
    public DateTime DataRegisto { get; set; } = DateTime.UtcNow;
}

// Um atributo de validação escrito por nós, para usar como [validarNIF].
// Herda de ValidationAttribute, a classe base de atributos como [Required]
// e [StringLength]. Basta reescrever o método IsValid: recebe o valor do campo
// e devolve sucesso ou uma mensagem de erro, que o formulário mostra.
public class validarNIF : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return new ValidationResult("O NIF é obrigatório.");
        }

        if (value is not long nif || nif.ToString().Length != 9)
        {
            return new ValidationResult("O NIF deve conter exatamente 9 dígitos.");
        }

        // Verificar se o NIF é válido (usando cálculo do dígito de controlo)
        if (!IsValidNIF(nif))
        {
            return new ValidationResult("O NIF é inválido.");
        }

        // Caso seja válido, retorna sucesso
        return ValidationResult.Success;
    }

    // O algoritmo de validação do NIF usado pelas Finanças (módulo 11).
    // O nono dígito é um dígito de controlo, calculado a partir dos oito primeiros:
    // se o cálculo não der o nono dígito, o número não é um NIF válido.
    private bool IsValidNIF(long nif)
    {
        string nifString = nif.ToString();

        // Primeiro dígito do NIF entre 1 e 9
        int primeiroDigito = int.Parse(nifString[0].ToString());
        if (primeiroDigito < 1 || primeiroDigito > 9)
        {
            return false;
        }

        // Calcular o dígito de controlo:
        // somar os oito primeiros dígitos multiplicados por 9, 8, 7, 6, 5, 4, 3 e 2
        int total = 0;

        for (int i = 0; i < 8; i++)
        {
            total += int.Parse(nifString[i].ToString()) * (9 - i);
        }

        // O dígito de controlo é 11 menos o resto da divisão da soma por 11.
        // Se der 10 ou 11, o dígito de controlo é 0.
        int digitoDeControlo = 11 - (total % 11);
        if (digitoDeControlo >= 10)
        {
            digitoDeControlo = 0;
        }

        // O NIF é válido se o dígito calculado for igual ao nono dígito.
        return digitoDeControlo == int.Parse(nifString[8].ToString());
    }
}