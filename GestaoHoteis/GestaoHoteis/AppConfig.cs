namespace GestaoHoteis;

// Os textos fixos da aplicação, num só sítio: nome da empresa, morada e contactos.
// A classe é static: não se criam objetos dela, usa-se diretamente, como em
// AppConfig.tituloHomePage (numa página Razor: @AppConfig.tituloHomePage).
// Está no namespace GestaoHoteis, a raiz do projeto, que o _Imports.razor já inclui:
// por isso fica disponível em todos os componentes sem nenhum @using a mais.
public static class AppConfig
{
    // Em cada campo:
    // static = pertence à classe, não a um objeto;
    // readonly = o valor é dado aqui e não pode ser alterado depois.

    // O nome da empresa. Aparece no topo do menu (Etapa 3).
    public static readonly string tituloHomePage = "MyHotels";

    // A morada da sede, mostrada na página Sobre (Etapa 3).
    public static readonly string enderecoHome = "Rua dos Viajantes 10, 3000-100 Coimbra, Portugal";

    // O email de contacto.
    public static readonly string mail = "geral@myhotels.pt";

    // O telefone de contacto.
    public static readonly string telefone = "239000000";
}