namespace RESTfulAPI.Services;

// Os tipos de erro que um Service pode devolver. Não são status codes:
// o Service não sabe de HTTP. É o Controller que os traduz (404, 400, 409).
// enum: uma lista fechada de valores com nome.
public enum TipoErro
{
    // Correu bem.
    Nenhum,
    // O recurso não existe → o controller responde 404.
    NaoEncontrado,
    // Os dados não servem (por exemplo, uma categoria que não existe) → 400.
    Invalido,
    // Choca com os dados que já existem (nome repetido, categoria com alojamentos) → 409.
    Conflito
}

// O resultado de uma operação sem valor de retorno (PUT, DELETE).
// Se falhou: o tipo de erro, um código estável para a app (ex.: "ALOJAMENTO_NOME_REPETIDO")
// e uma mensagem em português, pronta a mostrar ao utilizador.
// Porquê um Resultado e não uma exceção? Um nome repetido não é um acidente:
// é uma resposta normal, que se devolve como qualquer outra.
public class Resultado
{
    // init: só se pode dar valor ao criar o objeto (depois fica fixo).
    public TipoErro Erro { get; init; }
    public string? Codigo { get; init; }
    public string? Mensagem { get; init; }

    // Calculado: é sucesso se não houver erro.
    public bool Sucesso => Erro == TipoErro.Nenhum;

    // Métodos "fábrica": em vez de new Resultado { ... }, escreve-se Resultado.Conflito(...).
    // Lê-se melhor, e não há como esquecer o código ou a mensagem.
    public static Resultado Ok() => new();
    public static Resultado NaoEncontrado(string codigo, string mensagem) => new() { Erro = TipoErro.NaoEncontrado, Codigo = codigo, Mensagem = mensagem };
    public static Resultado Invalido(string codigo, string mensagem) => new() { Erro = TipoErro.Invalido, Codigo = codigo, Mensagem = mensagem };
    public static Resultado Conflito(string codigo, string mensagem) => new() { Erro = TipoErro.Conflito, Codigo = codigo, Mensagem = mensagem };
}

// O resultado de uma operação que devolve um valor (GET, POST): o DTO, se correu bem.
// <T> é um tipo genérico: Resultado<AlojamentoDto> leva um AlojamentoDto, Resultado<CategoriaDto> uma categoria.
// Herda de Resultado: tem também o Erro, o Codigo e a Mensagem.
public class Resultado<T> : Resultado
{
    // O valor devolvido (por exemplo, o AlojamentoDto). Só tem valor se Sucesso for true.
    public T? Valor { get; init; }

    // As mesmas fábricas, a devolver Resultado<T>. O "new" esconde as da classe base,
    // que devolvem Resultado sem valor.
    public static Resultado<T> Ok(T valor) => new() { Valor = valor };
    public static new Resultado<T> NaoEncontrado(string codigo, string mensagem) => new() { Erro = TipoErro.NaoEncontrado, Codigo = codigo, Mensagem = mensagem };
    public static new Resultado<T> Invalido(string codigo, string mensagem) => new() { Erro = TipoErro.Invalido, Codigo = codigo, Mensagem = mensagem };
    public static new Resultado<T> Conflito(string codigo, string mensagem) => new() { Erro = TipoErro.Conflito, Codigo = codigo, Mensagem = mensagem };
}
