using Microsoft.EntityFrameworkCore;
using RESTfulAPI.Data;
using RESTfulAPI.Entities;

namespace RESTfulAPI.Repositories;

// O acesso à tabela Alojamentos: cumpre o IAlojamentoRepository.
// Diferença para os outros: os alojamentos vêm sempre com as três referências e as duas
// coleções N-M (Include), porque o AlojamentoMapper.ToDto precisa delas.
public class AlojamentoRepository(AppDbContext context) : IAlojamentoRepository
{
    // A lista, com filtros opcionais (null = sem esse filtro).
    public async Task<List<Alojamento>> GetAllAsync(int? hotelId, int? categoriaId, int? estrelas, string? localidade, string? pesquisa)
    {
        // A consulta vai-se construindo; o SQL só é gerado e enviado no ToListAsync.
        // IQueryable: uma consulta ainda por executar, à qual se podem juntar mais condições.
        IQueryable<Alojamento> consulta = context.Alojamentos
            .AsNoTracking()
            .Include(a => a.hotel)                  // JOIN com Hoteis
            .Include(a => a.categoria)              // JOIN com Categorias
            .Include(a => a.modoDisponibilizacao)   // JOIN com ModosDisponibilizacao
            // Include filtrado: nas coleções, só os regimes e as características ativos (RN24, RN35).
            .Include(a => a.regimes.Where(r => r.Ativo))
            .Include(a => a.caracteristicas.Where(c => c.Ativo))
            // Só alojamentos ativos, de unidades ativas (RN04 e RN11).
            .Where(a => a.Ativo && a.hotel.Ativo);

        // Cada filtro que vier junta um WHERE à consulta (RN19).
        if (hotelId is not null)
            consulta = consulta.Where(a => a.HotelId == hotelId);

        if (categoriaId is not null)
            consulta = consulta.Where(a => a.CategoriaId == categoriaId);

        // Uma navegação dentro do Where: o EF Core faz o JOIN sozinho, sem Include.
        if (estrelas is not null)
            consulta = consulta.Where(a => a.hotel.Estrelas == estrelas);

        // Duas navegações seguidas: alojamento → unidade → morada.
        if (!string.IsNullOrWhiteSpace(localidade))
            consulta = consulta.Where(a => a.hotel.morada.Localidade!.Contains(localidade));

        // Pesquisa no nome ou na descrição: Contains passa a LIKE '%texto%' no SQL.
        // O "!" diz ao compilador que Nome e Descricao não são null (são obrigatórios).
        if (!string.IsNullOrWhiteSpace(pesquisa))
            consulta = consulta.Where(a => a.Nome!.Contains(pesquisa) || a.Descricao!.Contains(pesquisa));

        // Só aqui a consulta vai à base de dados.
        return await consulta.OrderBy(a => a.hotel.Nome).ThenBy(a => a.Nome).ToListAsync();
    }

    // Um alojamento, com tudo. Com tracking: o PUT e o DELETE usam o objeto devolvido,
    // e o PUT acerta as duas coleções (o EF Core compara-as com as que leu aqui).
    public Task<Alojamento?> GetByIdAsync(int id) =>
        context.Alojamentos
            .Include(a => a.hotel)
            .Include(a => a.categoria)
            .Include(a => a.modoDisponibilizacao)
            .Include(a => a.regimes)
            .Include(a => a.caracteristicas)
            .FirstOrDefaultAsync(a => a.Id == id);

    // Nome repetido na mesma unidade? (o índice único é sobre HotelId e Nome)
    public Task<bool> NomeExisteAsync(int hotelId, string nome, int? excetoId = null) =>
        context.Alojamentos.AnyAsync(a => a.HotelId == hotelId && a.Nome == nome.Trim() && a.Id != excetoId);

    // Há alguma linha de reserva com este alojamento? Pergunta-se à tabela LinhasReserva.
    public Task<bool> TemReservasAsync(int id) =>
        context.LinhasReserva.AnyAsync(l => l.AlojamentoId == id);

    // INSERT: marcar como novo e gravar. O EF Core insere também as linhas das duas tabelas
    // de ligação, uma por cada regime e característica da coleção. Depois disto, alojamento.Id tem o id novo.
    public async Task<bool> AddAsync(Alojamento alojamento)
    {
        context.Alojamentos.Add(alojamento);
        return await GravarAsync();
    }

    // UPDATE: o objeto já está a ser seguido (veio do GetByIdAsync), basta gravar.
    // Nas tabelas de ligação, o EF Core apaga as ligações que saíram e insere as que entraram.
    public Task<bool> UpdateAsync(Alojamento alojamento) => GravarAsync();

    // DELETE: marcar para apagar e gravar. As ligações aos regimes e às características
    // saem com ele (as duas tabelas de ligação têm cascata).
    public async Task<bool> DeleteAsync(Alojamento alojamento)
    {
        context.Alojamentos.Remove(alojamento);
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
