using RESTfulAPI.Entities;

namespace RESTfulAPI.Repositories;

// O que se pode pedir à "despensa" sobre categorias. Fala só de entidades.
// Uma interface é um contrato: diz o que existe, não como se faz.
// O CategoriaService só conhece esta interface; a classe CategoriaRepository é que a cumpre,
// e é o Program.cs (AddScoped) que diz ao ASP.NET Core qual classe usar.
// Todos os métodos são assíncronos (Task): enquanto a base de dados responde, o servidor
// pode tratar de outros pedidos.
public interface ICategoriaRepository
{
    // As categorias ativas, por nome (a listagem para as apps).
    Task<List<Categoria>> GetAllAsync();

    // Uma categoria, ativa ou não, ou null se não existir (o "?" em Categoria?).
    Task<Categoria?> GetByIdAsync(int id);

    // Existe uma categoria com este id? (só pergunta, não traz a linha)
    Task<bool> ExisteAsync(int id);

    // Já há outra categoria com este nome? excetoId serve para o PUT (ignorar a própria).
    Task<bool> NomeExisteAsync(string nome, int? excetoId = null);

    // A categoria tem alojamentos? (se tiver, não se pode apagar: RN18)
    Task<bool> TemAlojamentosAsync(int id);

    // Gravações: devolvem false se a base de dados recusar (por exemplo, o índice único do nome).
    Task<bool> AddAsync(Categoria categoria);
    Task<bool> UpdateAsync(Categoria categoria);
    Task<bool> DeleteAsync(Categoria categoria);
}
