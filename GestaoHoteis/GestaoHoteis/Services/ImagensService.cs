using GestaoHoteis.Interfaces;

namespace GestaoHoteis.Services;

// A implementação do serviço de imagens: guarda as imagens na base de dados (byte[]).
// Registada no Program.cs como Singleton: não guarda dados de nenhum utilizador,
// por isso um só objeto serve a aplicação inteira.
public class ImagensService : IImagensService
{
    // 500 KB: o limite que o InputFile usa por omissão.
    public long TamanhoMaximo => 512_000;

    // Imagem mostrada quando o registo não tem imagem (Etapa 1, alínea 3.n).
    private const string SemImagem = "/img/noproductstrans.png";

    // Extensões de ficheiro aceites.
    private static readonly string[] Extensoes = [".png", ".jpg", ".jpeg"];

    public string? Valida(string nomeFicheiro, long tamanho)
    {
        var extensao = Path.GetExtension(nomeFicheiro).ToLowerInvariant();
        if (!Extensoes.Contains(extensao))
        {
            return "Só são aceites imagens .png, .jpg ou .jpeg.";
        }

        if (tamanho > TamanhoMaximo)
        {
            return $"A imagem tem {tamanho / 1024} KB. O máximo são {TamanhoMaximo / 1024} KB.";
        }

        return null;
    }

    public async Task<byte[]> LerBytesAsync(Stream conteudo)
    {
        using var memoria = new MemoryStream();
        await conteudo.CopyToAsync(memoria);
        return memoria.ToArray();
    }

    public string Src(byte[]? imagem, string? nomeFicheiro)
    {
        if (imagem is null || imagem.Length == 0)
        {
            return SemImagem;
        }

        // O tipo da imagem deduz-se da extensão do nome guardado em UrlImagem.
        var tipo = Path.GetExtension(nomeFicheiro)?.ToLowerInvariant() == ".png"
            ? "image/png"
            : "image/jpeg";

        // Base64: os bytes escritos como texto, para caberem dentro do HTML.
        return $"data:{tipo};base64,{Convert.ToBase64String(imagem)}";
    }
}