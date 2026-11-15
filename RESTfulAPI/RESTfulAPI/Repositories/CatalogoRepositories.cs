using Microsoft.EntityFrameworkCore;
using RESTfulAPI.Data;
using RESTfulAPI.Entities;

namespace RESTfulAPI.Repositories;

// O acesso às tabelas RegimesAlimentares e Caracteristicas. Duas classes pequenas no mesmo ficheiro:
// só leituras, e o mesmo desenho.
public class RegimeAlimentarRepository(AppDbContext context) : IRegimeAlimentarRepository
{
    public Task<List<RegimeAlimentar>> GetAllAsync() =>
        context.RegimesAlimentares.AsNoTracking().Where(r => r.Ativo).OrderBy(r => r.Percentagem).ToListAsync();

    public Task<RegimeAlimentar?> GetByIdAsync(int id) =>
        context.RegimesAlimentares.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);

    // Sem AsNoTracking: estes objetos vão ser ligados a um alojamento, no mesmo contexto.
    // Contains passa a WHERE Id IN (...) no SQL.
    public Task<List<RegimeAlimentar>> GetByIdsAsync(IEnumerable<int> ids) =>
        context.RegimesAlimentares.Where(r => ids.Contains(r.Id)).ToListAsync();
}

public class CaracteristicaRepository(AppDbContext context) : ICaracteristicaRepository
{
    public Task<List<Caracteristica>> GetAllAsync() =>
        context.Caracteristicas.AsNoTracking().Where(c => c.Ativo).OrderBy(c => c.Nome).ToListAsync();

    public Task<Caracteristica?> GetByIdAsync(int id) =>
        context.Caracteristicas.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

    public Task<List<Caracteristica>> GetByIdsAsync(IEnumerable<int> ids) =>
        context.Caracteristicas.Where(c => ids.Contains(c.Id)).ToListAsync();
}
