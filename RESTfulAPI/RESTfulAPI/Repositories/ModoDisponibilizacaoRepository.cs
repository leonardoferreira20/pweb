using Microsoft.EntityFrameworkCore;
using RESTfulAPI.Data;
using RESTfulAPI.Entities;

namespace RESTfulAPI.Repositories;

// O acesso à tabela ModosDisponibilizacao: cumpre o IModoDisponibilizacaoRepository.
// O mesmo desenho do CategoriaRepository; o AppDbContext chega pelo construtor primário.
public class ModoDisponibilizacaoRepository(AppDbContext context, ILogger<ModoDisponibilizacaoRepository> logger) : IModoDisponibilizacaoRepository
{
    // Listagem só para ler: AsNoTracking, ordenada por nome.
    public Task<List<ModoDisponibilizacao>> GetAllAsync() =>
        context.ModosDisponibilizacao.AsNoTracking().OrderBy(m => m.Nome).ToListAsync();

    // Com tracking, porque o PUT e o DELETE alteram ou apagam o objeto devolvido.
    public Task<ModoDisponibilizacao?> GetByIdAsync(int id) =>
        context.ModosDisponibilizacao.FirstOrDefaultAsync(m => m.Id == id);

    // Só pergunta se existe (SELECT ... EXISTS).
    public Task<bool> ExisteAsync(int id) =>
        context.ModosDisponibilizacao.AnyAsync(m => m.Id == id);

    // Nome repetido? excetoId: no PUT, o próprio não conta.
    public Task<bool> NomeExisteAsync(string nome, int? excetoId = null) =>
        context.ModosDisponibilizacao.AnyAsync(m => m.Nome == nome.Trim() && m.Id != excetoId);

    // Há alojamentos com este modo? Pergunta-se à tabela Alojamentos.
    public Task<bool> TemAlojamentosAsync(int id) =>
        context.Alojamentos.AnyAsync(a => a.ModoDisponibilizacaoId == id);

    // INSERT: marcar como novo e gravar.
    public async Task<bool> AddAsync(ModoDisponibilizacao modo)
    {
        context.ModosDisponibilizacao.Add(modo);
        return await GravarAsync();
    }

    // UPDATE: o objeto já está a ser seguido, basta gravar.
    public Task<bool> UpdateAsync(ModoDisponibilizacao modo) => GravarAsync();

    // DELETE: marcar para apagar e gravar.
    public async Task<bool> DeleteAsync(ModoDisponibilizacao modo)
    {
        context.ModosDisponibilizacao.Remove(modo);
        return await GravarAsync();
    }

    // SaveChangesAsync gera o INSERT, UPDATE ou DELETE. Se a base de dados o recusar
    // (índice único, restrição CHECK, chave estrangeira), devolve false em vez de rebentar.
    // Escrito uma vez, usado pelas três gravações.
    private async Task<bool> GravarAsync()
    {
        try
        {
            await context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex)
        {
            // A exceção não sobe (o Service decide o que dizer à app), mas fica registada
            // no log, com o erro do SQL Server: sem isto, perdia-se a causa.
            logger.LogWarning(ex, "A base de dados recusou a gravação.");
            return false;
        }
    }
}
