using Microsoft.EntityFrameworkCore;
using RESTfulAPI.Data;
using RESTfulAPI.Entities;

namespace RESTfulAPI.Repositories;

// O único sítio onde se usa o AppDbContext para as categorias: cumpre o ICategoriaRepository.
// O que está entre parênteses no nome da classe é o construtor primário: o ASP.NET Core
// cria o repository e passa-lhe o AppDbContext (injeção de dependências, registada no Program.cs).
// Cada método é uma consulta LINQ, que o EF Core traduz em SQL.
public class CategoriaRepository(AppDbContext context) : ICategoriaRepository
{
    // Listagem: só as ativas (RN17), por nome. AsNoTracking, porque é só para ler
    // (o EF Core não guarda cópias, é mais leve).
    // SELECT ... FROM Categorias WHERE Ativo = 1 ORDER BY Nome
    public Task<List<Categoria>> GetAllAsync() =>
        context.Categorias
            .AsNoTracking()
            .Where(c => c.Ativo)
            .OrderBy(c => c.Nome)
            .ToListAsync();

    // Com tracking: o PUT e o DELETE alteram ou apagam o objeto devolvido.
    // FirstOrDefaultAsync devolve a primeira que encontrar, ou null se não houver nenhuma.
    public Task<Categoria?> GetByIdAsync(int id) =>
        context.Categorias.FirstOrDefaultAsync(c => c.Id == id);

    // AnyAsync: SELECT CASE WHEN EXISTS(...). Só pergunta se existe; não traz a linha.
    public Task<bool> ExisteAsync(int id) =>
        context.Categorias.AnyAsync(c => c.Id == id);

    // excetoId: no PUT, a própria categoria não conta como repetida.
    // No POST, excetoId é null, e "c.Id != null" é verdade para todas.
    public Task<bool> NomeExisteAsync(string nome, int? excetoId = null) =>
        context.Categorias.AnyAsync(c => c.Nome == nome.Trim() && c.Id != excetoId);

    // Há algum alojamento desta categoria? Pergunta-se à tabela Alojamentos.
    public Task<bool> TemAlojamentosAsync(int id) =>
        context.Alojamentos.AnyAsync(a => a.CategoriaId == id);

    // Add marca a categoria como nova; o INSERT só acontece no SaveChangesAsync.
    // Depois de gravar, o EF Core escreve no objeto o Id que a base de dados deu.
    public async Task<bool> AddAsync(Categoria categoria)
    {
        context.Categorias.Add(categoria);
        return await GravarAsync();
    }

    // A entidade já está a ser seguida pelo EF Core (veio do GetByIdAsync): basta gravar.
    // O EF Core compara-a com a cópia que guardou e gera o UPDATE só com o que mudou.
    public Task<bool> UpdateAsync(Categoria categoria) => GravarAsync();

    // Remove marca a categoria para apagar; o DELETE só acontece no SaveChangesAsync.
    public async Task<bool> DeleteAsync(Categoria categoria)
    {
        context.Categorias.Remove(categoria);
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
        catch (DbUpdateException)
        {
            return false;
        }
    }
}
