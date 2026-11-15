namespace RESTfulAPI.Erros;

// Os códigos de erro da API: fazem parte do contrato com a app frontend.
// A app decide o que fazer pelo código (que nunca muda); mostra ao utilizador o "detail" (que pode mudar).
// Exemplo: se receber "CATEGORIA_COM_ALOJAMENTOS", a app pode oferecer-se para mostrar os alojamentos.
// São constantes (const): escreve-se CodigosErro.AlojamentoNomeRepetido, e um engano no nome
// dá erro de compilação, em vez de um texto errado que só se descobre na app.
public static class CodigosErro
{
    // Gerais: postos pelo ErrosApi quando o erro não traz um código mais específico.
    // 400: um ou mais campos não passaram nas validações (vem com "errors").
    public const string DadosInvalidos = "DADOS_INVALIDOS";
    // 400: o corpo do pedido nem sequer é JSON válido.
    public const string JsonInvalido = "JSON_INVALIDO";
    // 401: falta o login (etapa seguinte).
    public const string NaoAutenticado = "NAO_AUTENTICADO";
    // 403: há login, mas sem permissão (etapa seguinte).
    public const string SemPermissao = "SEM_PERMISSAO";
    // 404: o endereço não existe.
    public const string NaoEncontrado = "NAO_ENCONTRADO";
    // 405: o endereço existe, mas não aceita este verbo (por exemplo, PATCH).
    public const string MetodoNaoPermitido = "METODO_NAO_PERMITIDO";
    // 409: o pedido choca com os dados que já existem.
    public const string Conflito = "CONFLITO";
    // 415: o corpo não vem como JSON (falta o Content-Type: application/json).
    public const string FormatoNaoSuportado = "FORMATO_NAO_SUPORTADO";
    // Outro 4xx sem código próprio.
    public const string PedidoInvalido = "PEDIDO_INVALIDO";
    // 500: um erro inesperado (uma exceção). Os detalhes ficam só na consola da API.
    public const string ErroInterno = "ERRO_INTERNO";

    // Categorias
    // 404: GET, PUT ou DELETE de uma categoria que não existe.
    public const string CategoriaNaoEncontrada = "CATEGORIA_NAO_ENCONTRADA";
    // 409: já há uma categoria com esse nome.
    public const string CategoriaNomeRepetido = "CATEGORIA_NOME_REPETIDO";
    // 409: não se apaga uma categoria que tem alojamentos (RN18).
    public const string CategoriaComAlojamentos = "CATEGORIA_COM_ALOJAMENTOS";
    // 400: um alojamento aponta para uma categoria que não existe.
    public const string CategoriaInexistente = "CATEGORIA_INEXISTENTE";

    // Modos de disponibilização (o mesmo raciocínio das categorias)
    public const string ModoNaoEncontrado = "MODO_NAO_ENCONTRADO";
    public const string ModoNomeRepetido = "MODO_NOME_REPETIDO";
    // 409: não se apaga um modo usado por alojamentos (RN30).
    public const string ModoEmUso = "MODO_EM_USO";
    public const string ModoInexistente = "MODO_INEXISTENTE";

    // Alojamentos
    public const string AlojamentoNaoEncontrado = "ALOJAMENTO_NAO_ENCONTRADO";
    // 409: já há, na mesma unidade, um alojamento com esse nome.
    public const string AlojamentoNomeRepetido = "ALOJAMENTO_NOME_REPETIDO";
    // 409: não se apaga um alojamento que aparece em reservas (RN12).
    public const string AlojamentoComReservas = "ALOJAMENTO_COM_RESERVAS";

    // Unidades hoteleiras, regimes e características (só leitura nesta etapa)
    public const string HotelNaoEncontrado = "HOTEL_NAO_ENCONTRADO";
    public const string HotelInexistente = "HOTEL_INEXISTENTE";
    public const string RegimeNaoEncontrado = "REGIME_NAO_ENCONTRADO";
    // 400: um dos regimes do alojamento não existe.
    public const string RegimeInexistente = "REGIME_INEXISTENTE";
    public const string CaracteristicaNaoEncontrada = "CARACTERISTICA_NAO_ENCONTRADA";
    public const string CaracteristicaInexistente = "CARACTERISTICA_INEXISTENTE";

    // Imagens (o campo "imagem" do POST e do PUT de alojamentos) e base de dados
    // 400: a imagem não é Base64 válido, ou não é uma imagem PNG, JPEG, GIF ou WebP.
    public const string ImagemInvalida = "IMAGEM_INVALIDA";
    // 400: a imagem tem mais de 2 MB.
    public const string ImagemDemasiadoGrande = "IMAGEM_DEMASIADO_GRANDE";
    // 409: a base de dados recusou a gravação (raro: dois pedidos ao mesmo tempo, por exemplo).
    public const string GravacaoRecusada = "GRAVACAO_RECUSADA";
}
