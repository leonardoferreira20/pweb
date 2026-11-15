namespace RESTfulAPI.DTOs;

// Uma referência curta a outro recurso: só o id e o nome.
// Usada dentro do AlojamentoDto para a unidade, a categoria e o modo (objetos aninhados).
// No JSON fica assim: { "id": 1, "nome": "Hotel Atlântico" }.
/// <summary>Uma referência curta a outro recurso: só o id e o nome (por exemplo, a unidade de um alojamento).</summary>
public class ReferenciaDto
{
    // O id do recurso referido (por exemplo, o id da unidade hoteleira).
    /// <summary>O id do recurso referido.</summary>
    /// <example>1</example>
    public int Id { get; set; }

    // O nome, para a app o mostrar sem ter de fazer outro pedido.
    /// <summary>O nome, para a app o mostrar sem outro pedido.</summary>
    /// <example>Hotel Atlântico</example>
    public string Nome { get; set; } = "";
}
