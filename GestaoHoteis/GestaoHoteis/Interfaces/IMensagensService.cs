namespace GestaoHoteis.Interfaces;

// Uma mensagem para mostrar ao utilizador: o texto, e se é de sucesso ou de erro.
// Fica neste ficheiro porque é o que a interface abaixo guarda e devolve.
public record Mensagem(string Texto, bool Sucesso);

// O contrato do serviço de mensagens: guarda a mensagem de uma operação
// até a página seguinte a mostrar.
// Exemplo: o Create grava, guarda "Categoria criada." e navega para a listagem;
// o <Aviso> da listagem vai buscar a mensagem e mostra-a a verde.
public interface IMensagensService
{
    void Sucesso(string texto);
    void Erro(string texto);

    // Devolve a mensagem guardada e esquece-a: cada mensagem aparece uma só vez.
    Mensagem? Retira();
}