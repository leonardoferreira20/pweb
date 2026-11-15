using RESTfulAPI.Erros;

namespace RESTfulAPI.Services;

// Ajuda para a imagem que chega no JSON do POST e do PUT dos alojamentos
// (o campo "imagem", em Base64; o ASP.NET Core já o converteu em byte[]).
// Fica numa classe à parte para servir também, mais à frente, as unidades hoteleiras e os utilizadores.
public static class Imagens
{
    // O tamanho máximo de uma imagem: 2 MB. As imagens vão também no JSON das
    // listagens (em Base64), por isso convém que sejam pequenas.
    public const int TamanhoMaximo = 2 * 1024 * 1024;

    // Verifica a imagem que veio no DTO. Devolve o erro (um Resultado Invalido), ou null se servir.
    // A imagem do alojamento é obrigatória, mas isso já o verificou o [Required] do DTO.
    public static Resultado? Validar(byte[]? imagem)
    {
        if (imagem is null)
            return null;

        if (imagem.Length > TamanhoMaximo)
            return Resultado.Invalido(CodigosErro.ImagemDemasiadoGrande, "A imagem tem no máximo 2 MB.");

        if (Extensao(imagem) is null)
            return Resultado.Invalido(CodigosErro.ImagemInvalida, "A imagem tem de ser PNG, JPEG, GIF ou WebP.");

        return null;
    }

    // O nome que fica na coluna UrlImagem (a Gestão de Hotéis guarda lá o nome do ficheiro).
    // Pelo JSON só vêm os bytes, sem o nome do ficheiro: usa-se o recurso e a extensão,
    // por exemplo "alojamento.png". Sem imagem, fica null.
    public static string? NomeFicheiro(string recurso, byte[]? imagem) =>
        imagem is null ? null : $"{recurso}{Extensao(imagem)}";

    // Descobre o tipo da imagem pelos primeiros bytes: a "assinatura" que cada formato tem no início.
    // A app manda só os bytes (sem nome nem Content-Type), por isso é assim que se sabe se é uma imagem.
    // Devolve a extensão (".png", ".jpg"...), ou null se não for PNG, JPEG, GIF nem WebP.
    private static string? Extensao(byte[] b)
    {
        if (Comeca(b, 0x89, 0x50, 0x4E, 0x47)) return ".png";   // os bytes de "‰PNG"
        if (Comeca(b, 0xFF, 0xD8, 0xFF)) return ".jpg";         // JPEG
        if (Comeca(b, 0x47, 0x49, 0x46, 0x38)) return ".gif";   // os bytes de "GIF8"
        // WebP: "RIFF" no início e "WEBP" a partir do byte 8.
        if (Comeca(b, 0x52, 0x49, 0x46, 0x46) && b.Length >= 12
            && b[8] == 0x57 && b[9] == 0x45 && b[10] == 0x42 && b[11] == 0x50) return ".webp";
        return null;
    }

    // Os primeiros bytes de b são estes? "params": escrevem-se os bytes separados por vírgulas.
    private static bool Comeca(byte[] b, params byte[] assinatura) =>
        b.Length >= assinatura.Length && b.AsSpan(0, assinatura.Length).SequenceEqual(assinatura);
}
