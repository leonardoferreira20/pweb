using GestaoHoteis.Interfaces;

namespace GestaoHoteis.Services;

// Registada no Program.cs como Scoped. Nas páginas interativas, o âmbito (scope)
// é a ligação SignalR: há um objeto destes por separador do browser, e cada
// utilizador vê só as suas mensagens. É o tempo de vida certo para este estado.
public class MensagensService : IMensagensService
{
    private Mensagem? pendente;

    public void Sucesso(string texto) => pendente = new Mensagem(texto, true);

    public void Erro(string texto) => pendente = new Mensagem(texto, false);

    public Mensagem? Retira()
    {
        var mensagem = pendente;
        pendente = null;
        return mensagem;
    }
}