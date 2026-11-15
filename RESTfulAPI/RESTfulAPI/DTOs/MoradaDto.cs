namespace RESTfulAPI.DTOs;

// A morada de uma unidade hoteleira, dentro do HotelDto (objeto aninhado).
// Sem o Id: a morada não é um recurso com endereço próprio na API; só existe dentro da unidade.
/// <summary>A morada de uma unidade hoteleira.</summary>
public class MoradaDto
{
    /// <summary>Rua, número e andar.</summary>
    /// <example>Avenida do Mar 12</example>
    public string Rua { get; set; } = "";

    /// <summary>O código postal, no formato 1234-567.</summary>
    /// <example>3080-000</example>
    public string CodigoPostal { get; set; } = "";

    /// <summary>A localidade.</summary>
    /// <example>Figueira da Foz</example>
    public string Localidade { get; set; } = "";

    /// <summary>O distrito.</summary>
    /// <example>Coimbra</example>
    public string Distrito { get; set; } = "";

    /// <summary>O país.</summary>
    /// <example>Portugal</example>
    public string Pais { get; set; } = "";
}
