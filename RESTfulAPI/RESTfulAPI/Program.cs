using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RESTfulAPI.Data;
using RESTfulAPI.Erros;
using RESTfulAPI.Repositories;
using RESTfulAPI.Services;

// O ponto de partida da API. Tem duas metades:
//   1. antes do builder.Build(): registar os serviços (o que o ASP.NET Core sabe criar);
//   2. depois: o pipeline (por onde passa cada pedido HTTP, e por que ordem).
var builder = WebApplication.CreateBuilder(args);

// ---------- 1. Serviços: o que o contentor de injeção de dependências sabe criar ----------

// A base de dados: a mesma da Gestão de Hotéis (connection string no appsettings.json).
// "?? throw": se a connection string faltar, a API nem arranca, com uma mensagem clara.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Falta a connection string 'DefaultConnection' no appsettings.json.");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// Repositories e Services: uma interface, uma classe. AddScoped: um objeto por pedido HTTP.
// Quando um controller pede um IAlojamentoService, o ASP.NET Core cria um AlojamentoService,
// e para isso cria antes os repositories de que ele precisa, e o AppDbContext. Ninguém faz "new".
// Todos os repositories do mesmo pedido recebem o MESMO AppDbContext (Scoped).
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IModoDisponibilizacaoRepository, ModoDisponibilizacaoRepository>();
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IRegimeAlimentarRepository, RegimeAlimentarRepository>();
builder.Services.AddScoped<ICaracteristicaRepository, CaracteristicaRepository>();
builder.Services.AddScoped<IAlojamentoRepository, AlojamentoRepository>();

builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IModoDisponibilizacaoService, ModoDisponibilizacaoService>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IRegimeAlimentarService, RegimeAlimentarService>();
builder.Services.AddScoped<ICaracteristicaService, CaracteristicaService>();
builder.Services.AddScoped<IAlojamentoService, AlojamentoService>();

// Os controllers. O fornecedor de metadados faz os erros de validação usarem
// os nomes do JSON ("precoNoite") em vez dos nomes do C# ("PrecoNoite").
builder.Services.AddControllers(options =>
{
    options.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());

    // As mensagens dos valores que vêm no URL (rota e query string), em português.
    // Por exemplo, GET /api/alojamentos?estrelas=abc dava "The value 'abc' is not valid.";
    // assim dá "O valor 'abc' não é válido." (400, com o erro em "errors", na chave "estrelas").
    // "NonProperty": os parâmetros dos métodos (como o estrelas); os outros: as propriedades.
    var mensagens = options.ModelBindingMessageProvider;
    mensagens.SetNonPropertyAttemptedValueIsInvalidAccessor(valor => $"O valor '{valor}' não é válido.");
    mensagens.SetAttemptedValueIsInvalidAccessor((valor, campo) => $"O valor '{valor}' não é válido para {campo}.");
    mensagens.SetValueIsInvalidAccessor(valor => $"O valor '{valor}' não é válido.");
    mensagens.SetValueMustBeANumberAccessor(campo => $"{campo} tem de ser um número.");
    mensagens.SetValueMustNotBeNullAccessor(valor => "O valor não pode ser vazio.");
});

// O contrato de erros: todos os erros saem em ProblemDetails, com o formato do ErrosApi.
builder.Services.AddProblemDetails(options => options.CustomizeProblemDetails = ErrosApi.Personalizar);

// O Swagger (vem do template): lê os controllers e gera a documentação e a página de testes.
// SwaggerDoc: o título e a descrição da página.
// IncludeXmlComments: junta as descrições e os exemplos dos comentários /// dos controllers e dos DTOs.
// Esses comentários vão para um ficheiro XML (RESTfulAPI.xml), que o compilador gera por causa do
// <GenerateDocumentationFile> no RESTfulAPI.csproj; AppContext.BaseDirectory é a pasta bin onde fica.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RESTfulAPI · MyHotels",
        Version = "v1",
        Description = "Unidades hoteleiras, alojamentos, categorias, modos de disponibilização, regimes alimentares e características, em JSON. Os erros vêm em ProblemDetails, com um \"codigo\"."
    });
    var ficheiroXml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, ficheiroXml));
});

// A partir daqui já não se registam serviços: a aplicação fica montada.
var app = builder.Build();

// ---------- 2. Pipeline: por onde passa cada pedido, por esta ordem ----------

// Uma exceção não tratada vira um 500 em ProblemDetails, sem detalhes internos.
// O erro completo continua a aparecer na consola da API.
// Vem primeiro para apanhar as exceções de tudo o que está depois.
app.UseExceptionHandler();

// Respostas de erro sem corpo (rota inexistente, verbo errado...) ganham um corpo ProblemDetails.
app.UseStatusCodePages();

// O Swagger só em desenvolvimento: num servidor a sério, não se mostra a página de testes.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Um pedido por http:// é reencaminhado para https://.
app.UseHttpsRedirection();

// Verifica as permissões ([Authorize]). Ainda não há nenhuma: entram na etapa seguinte.
app.UseAuthorization();

// Liga cada pedido ao método do controller certo, pelas rotas ([Route], [HttpGet]...).
app.MapControllers();

// Antes de aceitar pedidos: a base de dados responde, e tem as tabelas que as entidades esperam?
// Se não, a API não arranca: a consola diz porquê (LogCritical), e o programa termina com o código 1.
if (!await VerificacaoBaseDados.ExecutarAsync(app))
{
    Environment.ExitCode = 1;
    return;
}

// Arranca o servidor (Kestrel) e fica à espera de pedidos.
app.Run();
