using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GestaoHoteis.Components;
using GestaoHoteis.Components.Account;
using GestaoHoteis.Data;
// Os nossos serviços (Etapa 3): as interfaces e as classes que as cumprem.
using GestaoHoteis.Interfaces;
using GestaoHoteis.Services;

// =====================================================================
// 1.ª PARTE: REGISTAR SERVIÇOS
// O que é registado em builder.Services fica disponível, por injeção
// de dependências, no resto da aplicação.
// =====================================================================
var builder = WebApplication.CreateBuilder(args);

// Os componentes Razor, com interatividade no servidor.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Autenticação: entrega a todos os componentes o estado "quem tem sessão iniciada"
// (é o que o AuthorizeView usa) e regista os serviços das páginas de conta do template.
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

// A sessão do utilizador é guardada num cookie.
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

// Base de dados: lê a connection string do appsettings.json e regista o contexto,
// indicando que o motor de base de dados é o SQL Server.
// AddDbContextFactory (Etapa 3, em vez de AddDbContext) regista duas coisas:
//   - IDbContextFactory<ApplicationDbContext>, que as páginas CRUD pedem com @inject;
//   - o próprio ApplicationDbContext (Scoped), que o Identity continua a usar.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Acrescentado pelo scaffolder (Etapa 3): deixa a QuickGrid consultar
// o EF Core de forma assíncrona (contar, paginar e ordenar na base de dados).
builder.Services.AddQuickGridEntityFrameworkAdapter();

// Em desenvolvimento, mostra uma página de ajuda quando há erros de base de dados.
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity: os serviços que gerem utilizadores, sessões e roles.
// AddIdentityCore regista o UserManager; AddSignInManager, o SignInManager;
// AddRoles, o RoleManager (acrescentado na alínea 3.i).
// AddEntityFrameworkStores liga-os ao nosso contexto e tem de vir depois de AddRoles.
// RequireConfirmedAccount = false (Etapa 3): quem se regista entra logo,
// sem confirmar o email. Só em desenvolvimento, com emails de teste; numa aplicação
// publicada volta a true, com um serviço de email a sério.
builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddSignInManager()
    .AddRoles<IdentityRole>()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// O template não envia emails a sério: este serviço não faz nada.
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// Dá acesso ao pedido HTTP em curso, e ao utilizador que o fez,
// a partir de qualquer classe (acrescentado na alínea 3.i).
builder.Services.AddHttpContextAccessor();

// Os nossos serviços (Etapa 3). Cada linha diz ao contentor: "quando uma página pedir
// a interface (à esquerda, pasta Interfaces), entrega um objeto da classe (à direita, pasta Services)".
// Singleton: um só objeto para toda a aplicação (não guarda dados de ninguém).
builder.Services.AddSingleton<IImagensService, ImagensService>();
// Scoped: um objeto por âmbito; nas páginas interativas, um por separador do browser.
builder.Services.AddScoped<IMensagensService, MensagensService>();

// =====================================================================
// 2.ª PARTE: CONSTRUIR A APLICAÇÃO
// A partir daqui os serviços registados existem e podem ser pedidos.
// =====================================================================
var app = builder.Build();

// Dados iniciais (roles e utilizador administrador): início (alínea 3.k)
// O UserManager e o RoleManager são serviços Scoped: vivem dentro de um scope,
// que normalmente é um pedido HTTP. No arranque não há pedido, por isso cria-se
// um scope à mão. O using liberta-o, e aos serviços, no fim do bloco.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Etapa 3: antes de usar a base de dados, confirmar que existe e que tem
        // todas as migrations aplicadas. Se não existir, contam todas como pendentes.
        // Sem esta verificação, a aplicação parava mais abaixo com um erro de SQL
        // ("Cannot open database", "Invalid column name") difícil de perceber.
        var context = services.GetRequiredService<ApplicationDbContext>();
        var pendentes = await context.Database.GetPendingMigrationsAsync();
        if (pendentes.Any())
        {
            throw new InvalidOperationException(
                "A base de dados não existe ou não está atualizada. Faltam as migrations: "
                + string.Join(", ", pendentes)
                + ". Correr Update-Database no Console do Gerenciador de Pacotes.");
        }

        // Pedir os dois serviços ao contentor de injeção de dependências.
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Criar os roles e o administrador, se ainda não existirem.
        await Inicializacao.CriaDadosIniciais(userManager, roleManager);

        // Etapa 3: o catálogo de exemplo, se ainda não houver unidades hoteleiras.
        // As imagens vêm de wwwroot/img/seed; WebRootPath é o caminho da pasta wwwroot.
        await Inicializacao.CriaDadosExemplo(context,
            Path.Combine(app.Environment.WebRootPath, "img", "seed"));

        // Extra do grupo: mais dados de demonstração (20 unidades, 30 alojamentos, ...).
        // Só corre uma vez, e depois dos dados de exemplo, de que precisa.
        await Inicializacao.CriaDadosDemonstracao(context,
            Path.Combine(app.Environment.WebRootPath, "img", "seed"));
    }
    catch (Exception)
    {
        // Não trata o erro: volta a lançá-lo. É o sítio para o registar num log.
        throw;
    }
}
// Dados iniciais: fim

// =====================================================================
// 3.ª PARTE: TRATAR OS PEDIDOS
// Cada pedido HTTP passa por estas linhas, pela ordem em que estão escritas.
// =====================================================================
if (app.Environment.IsDevelopment())
{
    // Em desenvolvimento: o endereço que a página de erro de base de dados
    // usa para aplicar migrations.
    app.UseMigrationsEndPoint();
}
else
{
    // Com a aplicação publicada: página de erro genérica, e HSTS,
    // que obriga o browser a usar HTTPS.
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    // Acrescentado pelo scaffolder (Etapa 3). Nas aulas corremos sempre em
    // desenvolvimento, por isso esta linha nunca corre; pode ficar ou ser apagada.
    app.UseMigrationsEndPoint();
}

// Reencaminha os pedidos HTTP para HTTPS.
app.UseHttpsRedirection();

// Entrega os ficheiros da pasta wwwroot (CSS, imagens) tal como estão.
app.UseStaticFiles();
// Protege os formulários contra pedidos forjados por outros sítios.
app.UseAntiforgery();

// Liga os endereços às páginas (componentes Razor), a partir do componente raiz App.
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Os endereços que as páginas de conta do template usam (login, logout, ...).
app.MapAdditionalIdentityEndpoints();

// Arranca a aplicação e fica à espera de pedidos.
// O código só passa desta linha quando a aplicação termina.
app.Run();