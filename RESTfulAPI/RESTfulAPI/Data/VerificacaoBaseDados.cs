using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace RESTfulAPI.Data;

// Verifica a base de dados ANTES de a API começar a aceitar pedidos.
// Sem isto, uma connection string errada só se descobre no primeiro pedido, com um 500
// (e a API parece estar a funcionar, porque o Swagger abre).
// Com isto, a API não arranca, e a consola diz porquê.
// Chamado no Program.cs, depois do builder.Build() e antes do app.Run().
public static class VerificacaoBaseDados
{
    // Devolve true se estiver tudo bem; false (com o erro na consola) se não.
    public static async Task<bool> ExecutarAsync(WebApplication app)
    {
        // O AppDbContext é Scoped (um por pedido). Aqui ainda não há pedidos,
        // por isso cria-se um "scope" à mão, só para esta verificação.
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("RESTfulAPI.BaseDados");

        // O servidor e a base de dados que estão na connection string, para a mensagem dizer onde se tentou ligar.
        var cs = new SqlConnectionStringBuilder(db.Database.GetConnectionString());
        var onde = $"servidor '{cs.DataSource}', base de dados '{cs.InitialCatalog}'";

        // 1. Consegue ligar-se? O servidor tem de responder, e a base de dados tem de existir.
        try
        {
            await db.Database.OpenConnectionAsync();
            await db.Database.CloseConnectionAsync();
        }
        catch (Exception ex)
        {
            // LogCritical: o nível mais alto. A exceção (ex) vai junto: tem a causa exata do SQL Server.
            logger.LogCritical(ex,
                "A API não arrancou: não foi possível ligar à base de dados ({Onde}). " +
                "A connection string 'DefaultConnection' do appsettings.json tem de ser a mesma da Gestão de Hotéis.", onde);
            return false;
        }

        // 2. As tabelas e as colunas são as que as entidades esperam?
        // Cada consulta pede todas as colunas da tabela (mesmo que a tabela esteja vazia):
        // se faltar uma tabela ou uma coluna, o SQL Server recusa logo.
        // O OrderBy evita o aviso do EF Core sobre "FirstOrDefault sem OrderBy".
        try
        {
            await db.Moradas.AsNoTracking().OrderBy(m => m.Id).FirstOrDefaultAsync();
            await db.Hoteis.AsNoTracking().OrderBy(h => h.Id).FirstOrDefaultAsync();
            await db.Categorias.AsNoTracking().OrderBy(c => c.Id).FirstOrDefaultAsync();
            await db.Caracteristicas.AsNoTracking().OrderBy(c => c.Id).FirstOrDefaultAsync();
            await db.ModosDisponibilizacao.AsNoTracking().OrderBy(m => m.Id).FirstOrDefaultAsync();
            await db.RegimesAlimentares.AsNoTracking().OrderBy(r => r.Id).FirstOrDefaultAsync();
            await db.Alojamentos.AsNoTracking().OrderBy(a => a.Id).FirstOrDefaultAsync();
            await db.Campanhas.AsNoTracking().OrderBy(c => c.Id).FirstOrDefaultAsync();
            await db.Reservas.AsNoTracking().OrderBy(r => r.Id).FirstOrDefaultAsync();
            await db.LinhasReserva.AsNoTracking().OrderBy(l => l.Id).FirstOrDefaultAsync();
            await db.Pagamentos.AsNoTracking().OrderBy(p => p.Id).FirstOrDefaultAsync();
            await db.Faturas.AsNoTracking().OrderBy(f => f.Id).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex,
                "A API não arrancou: a base de dados ({Onde}) não tem as tabelas ou as colunas que as entidades esperam. " +
                "Aplique as migrations na Gestão de Hotéis (Update-Database) e confirme que as entidades da API são iguais às dela.", onde);
            return false;
        }

        logger.LogInformation("Base de dados verificada ({Onde}).", onde);
        return true;
    }
}
