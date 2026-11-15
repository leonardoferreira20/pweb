using Microsoft.EntityFrameworkCore;
using RESTfulAPI.Data;
using RESTfulAPI.Entities;

namespace RESTfulAPI.Repositories;

// O acesso à tabela Hoteis: cumpre o IHotelRepository. Só leituras, todas com AsNoTracking.
// As unidades vêm sempre com a morada (Include), porque o HotelMapper.ToDto precisa dela.
public class HotelRepository(AppDbContext context) : IHotelRepository
{
    // SELECT ... FROM Hoteis INNER JOIN Moradas ... WHERE Ativo = 1 ORDER BY Nome
    public Task<List<Hotel>> GetAllAsync() =>
        context.Hoteis
            .AsNoTracking()
            .Include(h => h.morada)
            .Where(h => h.Ativo)
            .OrderBy(h => h.Nome)
            .ToListAsync();

    public Task<Hotel?> GetByIdAsync(int id) =>
        context.Hoteis
            .AsNoTracking()
            .Include(h => h.morada)
            .FirstOrDefaultAsync(h => h.Id == id);

    public Task<bool> ExisteAsync(int id) =>
        context.Hoteis.AnyAsync(h => h.Id == id);
}
