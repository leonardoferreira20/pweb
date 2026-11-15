namespace GestaoHoteis.Interfaces;

// O contrato do serviço de imagens: o que as páginas lhe podem pedir.
// As páginas só conhecem esta interface (@inject IImagensService Imagens).
// Qual é a classe concreta que a cumpre decide-o o Program.cs, ao registar o serviço.
public interface IImagensService
{
    // Tamanho máximo aceite, em bytes.
    long TamanhoMaximo { get; }

    // Confirma a extensão e o tamanho do ficheiro.
    // Devolve null se estiver tudo bem, ou a mensagem de erro a mostrar.
    string? Valida(string nomeFicheiro, long tamanho);

    // Lê o conteúdo de um ficheiro para um array de bytes (o que se guarda em Imagem).
    // Recebe um Stream, e não um IBrowserFile: assim serve também à API,
    // onde o ficheiro chega como IFormFile (que também dá um Stream).
    Task<byte[]> LerBytesAsync(Stream conteudo);

    // O texto a pôr no src de um <img>: a imagem guardada, ou a imagem "sem imagem".
    string Src(byte[]? imagem, string? nomeFicheiro);
}