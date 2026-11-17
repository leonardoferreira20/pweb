namespace GestaoHoteis;

// Os ícones que se podem escolher para as categorias, as características e os regimes
// alimentares (extra do grupo). Cada ícone é um ficheiro SVG em wwwroot/img/icones/<grupo>/.
// Na base de dados guarda-se só o nome do ficheiro, sem a extensão (por exemplo "wifi"),
// na coluna Icone. Assim a API devolve o mesmo nome, e o frontend usa os seus ficheiros.
// A classe é static, como a AppConfig: não guarda dados de ninguém, só a lista fixa.
public static class Icones
{
    // Os três grupos: cada um é o nome de uma pasta de wwwroot/img/icones.
    public const string Categorias = "categorias";
    public const string Caracteristicas = "caracteristicas";
    public const string Regimes = "regimes";

    // Por grupo, os ícones que existem: o nome do ficheiro e o texto que aparece ao escolher.
    // O primeiro de cada lista é o genérico, usado quando um item não tem ícone.
    public static readonly Dictionary<string, (string Nome, string Texto)[]> Lista = new()
    {
        [Categorias] =
        [
            ("categoria-generica", "Genérico"),
            ("quarto-individual", "Quarto individual"),
            ("quarto-duplo", "Quarto duplo"),
            ("quarto-familiar", "Quarto familiar"),
            ("suite", "Suite"),
            ("apartamento", "Apartamento"),
        ],
        [Caracteristicas] =
        [
            ("caracteristica-generica", "Genérico"),
            ("wifi", "Wi-Fi"),
            ("ar-condicionado", "Ar condicionado"),
            ("varanda", "Varanda"),
            ("vista-mar", "Vista mar"),
            ("vista-montanha", "Vista montanha"),
            ("vista-rio", "Vista rio"),
            ("kitchenette", "Kitchenette"),
            ("banheira", "Banheira"),
            ("lareira", "Lareira"),
            ("televisao", "Televisão"),
            ("frigorifico", "Frigorífico"),
            ("cofre", "Cofre"),
            ("estacionamento", "Estacionamento"),
            ("acessivel", "Acessível"),
            ("animais", "Aceita animais"),
        ],
        [Regimes] =
        [
            ("regime-generico", "Genérico"),
            ("sem-refeicoes", "Sem refeições"),
            ("pequeno-almoco", "Pequeno-almoço"),
            ("meia-pensao", "Meia pensão"),
            ("pensao-completa", "Pensão completa"),
        ],
    };

    // O endereço do ícone, para o src de um <img>. Sem ícone, ou com um nome que não
    // está na lista (por exemplo, escrito à mão na base de dados), devolve o genérico.
    public static string Src(string grupo, string? nome)
    {
        var lista = Lista[grupo];
        var existe = lista.Any(i => i.Nome == nome);
        return $"/img/icones/{grupo}/{(existe ? nome : lista[0].Nome)}.svg";
    }

    // O mesmo, mas a contar com um ícone carregado (a coluna IconeImagem): se o item tem uma
    // imagem própria, é ela, escrita em Base64 dentro do HTML, como as imagens dos alojamentos;
    // senão, o ícone da lista com esse nome.
    public static string Src(string grupo, string? nome, byte[]? imagem)
    {
        if (imagem is null || imagem.Length == 0)
        {
            return Src(grupo, nome);
        }
        return $"data:{TipoImagem(imagem)};base64,{Convert.ToBase64String(imagem)}";
    }

    // O tipo da imagem, pelos primeiros bytes: cada formato tem uma "assinatura" no início.
    // Na Gestão de Hotéis só se carregam PNG e JPEG; pela API também podem chegar GIF e WebP.
    private static string TipoImagem(byte[] b)
    {
        if (b.Length > 3 && b[0] == 0x89 && b[1] == 0x50 && b[2] == 0x4E && b[3] == 0x47) return "image/png";
        if (b.Length > 3 && b[0] == 0x47 && b[1] == 0x49 && b[2] == 0x46) return "image/gif";
        if (b.Length > 11 && b[8] == 0x57 && b[9] == 0x45 && b[10] == 0x42 && b[11] == 0x50) return "image/webp";
        return "image/jpeg";
    }
}
