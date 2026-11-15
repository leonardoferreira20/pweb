using Microsoft.AspNetCore.Mvc;
using RESTfulAPI.Erros;
using RESTfulAPI.Services;

namespace RESTfulAPI.Controllers;

// A base dos nossos controllers. Herda de ControllerBase (API, sem views)
// e acrescenta um método: traduzir um Resultado com erro num status code.
// abstract: não se usa sozinha; os controllers herdam dela (": ApiControllerBase").
// Assim a tradução está escrita uma vez, em vez de repetida em cada método.
public abstract class ApiControllerBase : ControllerBase
{
    // NaoEncontrado → 404, Conflito → 409, Invalido → 400.
    // protected: só os controllers que herdam desta classe o podem chamar.
    protected ActionResult Erro(Resultado resultado)
    {
        // Escolher o status code pelo tipo de erro; "_" é o caso que sobra (Invalido).
        var status = resultado.Erro switch
        {
            TipoErro.NaoEncontrado => StatusCodes.Status404NotFound,
            TipoErro.Conflito => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest
        };

        // O código específico do Service (ex.: "ALOJAMENTO_NOME_REPETIDO") fica no pedido,
        // para o ErrosApi o pôr na resposta (e, com o Extra de logging, no log).
        HttpContext.Items[ErrosApi.ChaveCodigo] = resultado.Codigo;

        // Problem() cria a resposta em ProblemDetails, com a mensagem do Service no "detail".
        // O título em português, o código e o traceId são postos pelo ErrosApi.
        return Problem(detail: resultado.Mensagem, statusCode: status);
    }
}
