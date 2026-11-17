using GestaoHoteis.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GestaoHoteis.Data;

// Cria os dados sem os quais a aplicação não se consegue usar:
// os três roles e um utilizador administrador.
// A classe é static: não guarda dados nem se criam objetos dela. Serve só para
// arrumar o método abaixo, que o Program.cs chama em cada arranque da aplicação.
public static class Inicializacao
{
    // Recebe os dois serviços do Identity que gerem utilizadores e roles.
    // Não os cria: é o Program.cs, que chama este método, que os entrega.
    // É async porque cada operação vai à base de dados; o await liberta a thread
    // enquanto se espera pela resposta.
    public static async Task CriaDadosIniciais(UserManager<ApplicationUser> userManager,
                                               RoleManager<IdentityRole> roleManager)
    {
        // Os roles por omissão: os três perfis de utilizador registado.
        // (O quarto perfil, Anónimo, é quem não tem sessão iniciada: não é um role.)
        string[] roles = ["Administrador", "Funcionario", "Cliente"];

        foreach (var role in roles)
        {
            // Só cria o role se ainda não existir. Este método corre em todos
            // os arranques e não pode criar os mesmos dados duas vezes.
            if (!await roleManager.RoleExistsAsync(role))
            {
                IdentityRole roleRole = new IdentityRole(role);
                await roleManager.CreateAsync(roleRole);
            }
        }

        // O utilizador por omissão: o administrador.
        // Aqui é só um objeto em memória; ainda nada foi gravado.
        var defaultUser = new ApplicationUser
        {
            UserName = "admin@localhost.com",
            Email = "admin@localhost.com",
            Nome = "Administrador",
            Apelido = "Local",
            // Só utilizadores no estado Ativo podem iniciar sessão (regras de negócio).
            Estado = EstadoUtilizador.Ativo,
            // Conta já confirmada: o administrador entra logo, mesmo que o Program.cs
            // volte a exigir confirmação (RequireConfirmedAccount = true).
            EmailConfirmed = true,
            PhoneNumberConfirmed = true
        };
        // Confirma que nenhum utilizador já gravado tem o Id deste objeto novo.
        if (userManager.Users.All(u => u.Id != defaultUser.Id))
        {
            // Só cria o administrador se ainda não houver ninguém com este email.
            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            if (user == null)
            {
                // Grava o utilizador. A palavra-passe não é guardada: guarda-se o seu hash.
                await userManager.CreateAsync(defaultUser, "Is3C..00");
                // Dá-lhe o role Administrador: uma linha na tabela AspNetUserRoles.
                await userManager.AddToRoleAsync(defaultUser, "Administrador");
            }
        }
    }

    // Povoamento do catálogo (Etapa 3): modos, regimes, categorias, características,
    // unidades hoteleiras, alojamentos e campanhas de exemplo, para não se perder tempo
    // a inseri-los à mão. Só corre se ainda não houver unidades: nunca duplica dados.
    // As imagens são lidas da pasta wwwroot/img/seed (Etapa3_imagens_exemplo.zip).
    public static async Task CriaDadosExemplo(ApplicationDbContext context, string pastaImagens)
    {
        if (await context.Hoteis.AnyAsync())
        {
            return;
        }

        // Lê uma imagem da pasta. Se o ficheiro não existir, o registo fica sem imagem.
        byte[]? Le(string ficheiro)
        {
            var caminho = Path.Combine(pastaImagens, ficheiro);
            return File.Exists(caminho) ? File.ReadAllBytes(caminho) : null;
        }

        // Modos de disponibilização: os dois das regras de negócio.
        var listagem = new ModoDisponibilizacao { Nome = "Listagem", Descricao = "Aparece na aplicação, mas não pode ser reservado" };
        var reserva = new ModoDisponibilizacao { Nome = "Reserva", Descricao = "Pode ser reservado" };
        context.ModosDisponibilizacao.AddRange(listagem, reserva);

        // Regimes alimentares, com o acréscimo sobre o preço por noite.
        // Icone (extra do grupo): o nome do ficheiro em wwwroot/img/icones, sem a extensão.
        var semRefeicoes = new RegimeAlimentar { Nome = "Sem refeições", Descricao = "Só alojamento", Percentagem = 0, Icone = "sem-refeicoes" };
        var pequenoAlmoco = new RegimeAlimentar { Nome = "Pequeno-almoço", Descricao = "Alojamento e pequeno-almoço", Percentagem = 10, Icone = "pequeno-almoco" };
        var meiaPensao = new RegimeAlimentar { Nome = "Meia pensão", Descricao = "Pequeno-almoço e jantar", Percentagem = 30, Icone = "meia-pensao" };
        var pensaoCompleta = new RegimeAlimentar { Nome = "Pensão completa", Descricao = "Pequeno-almoço, almoço e jantar", Percentagem = 45, Icone = "pensao-completa" };
        context.RegimesAlimentares.AddRange(semRefeicoes, pequenoAlmoco, meiaPensao, pensaoCompleta);

        // Categorias (tipologias).
        var individual = new Categoria { Nome = "Quarto Individual", Descricao = "Quarto para uma pessoa", Icone = "quarto-individual" };
        var duplo = new Categoria { Nome = "Quarto Duplo", Descricao = "Quarto para duas pessoas", Icone = "quarto-duplo" };
        var suite = new Categoria { Nome = "Suite", Descricao = "Quarto com zona de estar", Icone = "suite" };
        var t1 = new Categoria { Nome = "Apartamento T1", Descricao = "Apartamento com um quarto e kitchenette", Icone = "apartamento" };
        context.Categorias.AddRange(individual, duplo, suite, t1);

        // Características.
        var wifi = new Caracteristica { Nome = "Wi-Fi", Icone = "wifi" };
        var ac = new Caracteristica { Nome = "Ar condicionado", Icone = "ar-condicionado" };
        var varanda = new Caracteristica { Nome = "Varanda", Icone = "varanda" };
        var vistaMar = new Caracteristica { Nome = "Vista mar", Icone = "vista-mar" };
        var kitchenette = new Caracteristica { Nome = "Kitchenette", Icone = "kitchenette" };
        var banheira = new Caracteristica { Nome = "Banheira", Icone = "banheira" };
        context.Caracteristicas.AddRange(wifi, ac, varanda, vistaMar, kitchenette, banheira);

        // Unidades hoteleiras, cada uma com a sua morada (um objeto Morada novo para cada).
        // O EF Core grava primeiro a morada e põe o seu Id em MoradaId.
        var atlantico = new Hotel
        {
            Nome = "Hotel Atlântico",
            Tipo = TipoHotel.Hotel,
            Estrelas = 4,
            Descricao = "Hotel de praia, em frente ao mar, com piscina exterior.",
            UrlImagem = "hotel-atlantico.png",
            Imagem = Le("hotel-atlantico.png"),
            morada = new Morada { Rua = "Avenida do Mar 12", CodigoPostal = "3080-000", Localidade = "Figueira da Foz", Distrito = "Coimbra" }
        };
        var ribeira = new Hotel
        {
            Nome = "Aparthotel Ribeira",
            Tipo = TipoHotel.Aparthotel,
            Estrelas = 3,
            Descricao = "Apartamentos no centro histórico, junto ao rio.",
            UrlImagem = "aparthotel-ribeira.png",
            Imagem = Le("aparthotel-ribeira.png"),
            morada = new Morada { Rua = "Rua da Ribeira 45", CodigoPostal = "4050-000", Localidade = "Porto", Distrito = "Porto" }
        };
        var montanha = new Hotel
        {
            Nome = "Hotel Montanha",
            Tipo = TipoHotel.Hotel,
            Estrelas = 5,
            Descricao = "Hotel de montanha, com spa e lareira nas zonas comuns.",
            UrlImagem = "hotel-montanha.png",
            Imagem = Le("hotel-montanha.png"),
            morada = new Morada { Rua = "Estrada da Torre 3", CodigoPostal = "6200-000", Localidade = "Covilhã", Distrito = "Castelo Branco" }
        };
        context.Hoteis.AddRange(atlantico, ribeira, montanha);

        // Cria um alojamento. As navegações (hotel, categoria, modoDisponibilizacao) chegam:
        // o EF Core preenche as chaves estrangeiras. Os regimes e as características entram
        // nas duas coleções N-M, e o EF Core grava as linhas das tabelas de ligação.
        Alojamento A(string nome, string descricao, Hotel hotel, Categoria categoria, int capacidade, int unidades,
                     decimal preco, string imagem, RegimeAlimentar[] regimes, Caracteristica[] caracteristicas,
                     ModoDisponibilizacao? modo = null) => new()
                     {
                         Nome = nome,
                         Descricao = descricao,
                         hotel = hotel,
                         categoria = categoria,
                         modoDisponibilizacao = modo ?? reserva,
                         Capacidade = capacidade,
                         NumUnidades = unidades,
                         PrecoNoite = preco,
                         UrlImagem = imagem,
                         Imagem = Le(imagem),
                         regimes = regimes.ToList(),
                         caracteristicas = caracteristicas.ToList()
                     };

        context.Alojamentos.AddRange(
            A("Quarto Duplo Vista Mar", "Quarto duplo com varanda virada ao mar.", atlantico, duplo, 2, 12, 95.00m,
              "quarto-duplo-vista-mar.png", [semRefeicoes, pequenoAlmoco, meiaPensao], [wifi, ac, varanda, vistaMar]),
            A("Quarto Individual", "Quarto individual, virado ao jardim.", atlantico, individual, 1, 6, 60.00m,
              "quarto-individual.png", [semRefeicoes, pequenoAlmoco], [wifi, ac]),
            A("Suite Atlântico", "Suite com sala de estar e vista mar.", atlantico, suite, 3, 2, 180.00m,
              "suite-atlantico.png", [pequenoAlmoco, meiaPensao, pensaoCompleta], [wifi, ac, varanda, vistaMar, banheira]),
            A("Apartamento T1 Ribeira", "Apartamento T1 com kitchenette equipada.", ribeira, t1, 4, 8, 110.00m,
              "apartamento-t1.png", [semRefeicoes, pequenoAlmoco], [wifi, ac, kitchenette]),
            A("Apartamento T1 Premium", "Apartamento T1 renovado (em remodelação: só em listagem).", ribeira, t1, 4, 2, 150.00m,
              "apartamento-t1-premium.png", [semRefeicoes], [wifi, ac, kitchenette, varanda], listagem),
            A("Quarto Duplo Montanha", "Quarto duplo com vista para a serra.", montanha, duplo, 2, 20, 120.00m,
              "quarto-duplo-montanha.png", [pequenoAlmoco, meiaPensao, pensaoCompleta], [wifi, varanda]),
            A("Suite Lareira", "Suite com lareira e banheira de hidromassagem.", montanha, suite, 2, 4, 240.00m,
              "suite-lareira.png", [pequenoAlmoco, meiaPensao], [wifi, banheira])
        );

        // Campanhas: uma época alta (aumenta o preço) e um desconto.
        var ano = DateTime.Today.Year + 1;
        context.Campanhas.AddRange(
            new Campanha { Nome = "Época alta de verão", Descricao = "Julho e agosto", hotel = atlantico, Percentagem = 20, DataInicio = new DateTime(ano, 7, 1), DataFim = new DateTime(ano, 8, 31) },
            new Campanha { Nome = "Semana da cidade", Descricao = "Desconto de aniversário", hotel = ribeira, Percentagem = -10, DataInicio = new DateTime(ano, 3, 10), DataFim = new DateTime(ano, 3, 17) }
        );

        // Um só SaveChangesAsync: grava tudo, pela ordem certa, numa só transação.
        await context.SaveChangesAsync();
    }
}