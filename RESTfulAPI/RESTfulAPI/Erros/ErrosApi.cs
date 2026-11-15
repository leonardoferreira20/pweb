using Microsoft.AspNetCore.Http;

namespace RESTfulAPI.Erros;

// O formato de TODOS os erros da API (o contrato com o frontend):
//   ProblemDetails (RFC 9457) com título em português, "detail" pronto a mostrar ao utilizador
//   e um "codigo" estável para o código da app.
// Corre para os erros dos nossos controllers (Problem), para os 400 e 415 automáticos do [ApiController],
// para os 404/405 sem corpo (UseStatusCodePages) e para as exceções (UseExceptionHandler).
// Está ligado no Program.cs: AddProblemDetails(o => o.CustomizeProblemDetails = ErrosApi.Personalizar).
// Assim, o formato dos erros está escrito num só sítio, e nenhum controller o repete.
public static class ErrosApi
{
    // Onde o controller deixa o código específico do erro (ver ApiControllerBase.Erro).
    // HttpContext.Items é uma "mochila" que acompanha o pedido do princípio ao fim.
    public const string ChaveCodigo = "codigoErro";

    // Chamado pelo ASP.NET Core sempre que vai sair uma resposta de erro.
    // Recebe o ProblemDetails já montado (com o status) e acaba de o preencher.
    public static void Personalizar(ProblemDetailsContext contexto)
    {
        var pd = contexto.ProblemDetails;
        // Se não vier status, trata-se como erro interno (500).
        var status = pd.Status ?? StatusCodes.Status500InternalServerError;

        // Para cada status: o título, o código geral e o texto por omissão.
        // "switch" com "=>" escolhe a linha pelo valor de status; "_" é "qualquer outro".
        var (titulo, codigo, detalhe) = status switch
        {
            400 => ("Os dados enviados têm erros.", CodigosErro.DadosInvalidos, "Corrija os campos assinalados e tente outra vez."),
            401 => ("Não autenticado.", CodigosErro.NaoAutenticado, "É preciso iniciar sessão."),
            403 => ("Sem permissão.", CodigosErro.SemPermissao, "Não tem permissão para esta operação."),
            404 => ("Não encontrado.", CodigosErro.NaoEncontrado, "O recurso pedido não existe."),
            405 => ("Método não permitido.", CodigosErro.MetodoNaoPermitido, "Este endereço não aceita este verbo HTTP."),
            409 => ("Conflito com os dados existentes.", CodigosErro.Conflito, "O pedido choca com os dados que já existem."),
            415 => ("Formato não suportado.", CodigosErro.FormatoNaoSuportado, "O corpo do pedido tem de ser JSON (Content-Type: application/json)."),
            < 500 => ("Pedido inválido.", CodigosErro.PedidoInvalido, "O pedido não pode ser atendido."),
            _ => ("Erro interno do servidor.", CodigosErro.ErroInterno, "Ocorreu um erro inesperado. Tente mais tarde.")
        };

        // O título é sempre o nosso, em português.
        pd.Title = titulo;

        // O detalhe: o que veio (a mensagem do Service), ou o texto por omissão.
        // Num 500 nunca se mostra o que veio: podia revelar detalhes internos. O erro real fica na consola.
        if (status >= 500 || string.IsNullOrWhiteSpace(pd.Detail))
            pd.Detail = detalhe;

        // O código específico que o controller deixou (vem do Service), se houver.
        // Por exemplo "ALOJAMENTO_NOME_REPETIDO" em vez do geral "CONFLITO".
        if (contexto.HttpContext.Items[ChaveCodigo] is string codigoDoService)
            codigo = codigoDoService;

        // O corpo do pedido não se converteu no DTO. O ASP.NET Core marca-o com chaves "$..." (o caminho
        // do campo no JSON, por exemplo "$.precoNoite"), "" (corpo vazio) ou "dto" (o nome do parâmetro do
        // controller: o corpo veio null), e com mensagens técnicas em inglês, que não servem à app.
        if (pd is HttpValidationProblemDetails validacao
            && validacao.Errors.Keys.Any(k => k.StartsWith('$') || k == "" || k == "dto"))
        {
            // Um campo com o tipo errado ("precoNoite": "abc", "ativo": "sim", "imagem": "string"):
            // o JSON está bem escrito, e a mensagem do .NET diz "could not be converted".
            var campoErrado = validacao.Errors
                .Where(e => e.Key.StartsWith("$.") && e.Value.Any(m => m.Contains("could not be converted")))
                .Select(e => new { Campo = e.Key[2..], Mensagem = e.Value[0] })
                .FirstOrDefault();

            // As mensagens em inglês saem todas; ficam só as nossas (se houver).
            validacao.Errors.Clear();

            if (campoErrado is null)
            {
                // 1. JSON mal escrito (uma vírgula a mais, uma aspa em falta), corpo vazio ou null.
                codigo = CodigosErro.JsonInvalido;
                pd.Detail = "O corpo do pedido está vazio ou não é JSON válido.";
            }
            else if (campoErrado.Campo.Equals("imagem", StringComparison.OrdinalIgnoreCase))
            {
                // 2. A imagem não é Base64: o "string" que o Swagger põe por omissão,
                //    ou o Base64 com "data:image/png;base64," à frente.
                const string mensagem = "A imagem não está em Base64 válido. Envie só o texto Base64 (sem \"data:image/...;base64,\" à frente).";
                codigo = CodigosErro.ImagemInvalida;
                pd.Detail = mensagem;
                validacao.Errors["imagem"] = [mensagem];
            }
            else
            {
                // 3. Outro campo com o tipo errado: fica em "errors", como um erro de validação
                //    (o código continua DADOS_INVALIDOS), para a app o mostrar junto ao campo.
                validacao.Errors[campoErrado.Campo] = [TipoEsperado(campoErrado.Mensagem)];
            }
        }

        // Extensions: campos a mais no JSON do erro, além dos do ProblemDetails.
        pd.Extensions["codigo"] = codigo;

        // O traceId identifica o pedido: quem vê o erro na app pode indicá-lo a quem dá suporte.
        pd.Extensions["traceId"] = contexto.HttpContext.TraceIdentifier;
    }

    // A mensagem para um campo com o tipo errado, conforme o tipo que o DTO esperava.
    // O tipo vem na mensagem do .NET: "The JSON value could not be converted to System.Decimal...".
    private static string TipoEsperado(string mensagemDoNet) => mensagemDoNet switch
    {
        _ when mensagemDoNet.Contains("System.Boolean") => "Tem de ser true ou false, sem aspas.",
        _ when mensagemDoNet.Contains("System.Int32") => "Tem de ser um número inteiro, sem aspas.",
        _ when mensagemDoNet.Contains("System.Decimal") => "Tem de ser um número, com ponto e sem aspas (por exemplo 95.50).",
        _ when mensagemDoNet.Contains("System.String") => "Tem de ser texto, entre aspas.",
        _ => "O valor não é do tipo certo."
    };
}
