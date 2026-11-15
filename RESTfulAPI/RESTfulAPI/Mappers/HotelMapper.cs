using RESTfulAPI.DTOs;
using RESTfulAPI.Entities;

namespace RESTfulAPI.Mappers;

// Converte a entidade Hotel (e a sua Morada) no HotelDto. Só ToDto: a API só lê as unidades.
public static class HotelMapper
{
    // Entidade → DTO (GET).
    // Precisa da morada carregada (o Include no Repository): sem o Include, h.morada seria null
    // e o ToDto da morada rebentava.
    public static HotelDto ToDto(this Hotel h) => new()
    {
        Id = h.Id,
        Nome = h.Nome ?? "",
        Tipo = h.Tipo,
        Estrelas = h.Estrelas,
        Descricao = h.Descricao ?? "",
        Ativo = h.Ativo,
        // A morada vai aninhada: o ToDto da Morada, logo abaixo.
        Morada = h.morada.ToDto(),
        Imagem = h.Imagem
    };

    // Entidade → DTO da morada (dentro do HotelDto).
    public static MoradaDto ToDto(this Morada m) => new()
    {
        Rua = m.Rua ?? "",
        CodigoPostal = m.CodigoPostal ?? "",
        Localidade = m.Localidade ?? "",
        Distrito = m.Distrito ?? "",
        Pais = m.Pais ?? ""
    };
}
