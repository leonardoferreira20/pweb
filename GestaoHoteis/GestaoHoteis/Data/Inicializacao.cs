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

    // Dados de demonstração (extra do grupo): mais 20 unidades hoteleiras, 30 alojamentos
    // (um por cada fotografia de wwwroot/img/seed), mais categorias, características, um regime
    // e campanhas, para as listagens, os filtros e a paginação terem com que trabalhar.
    // Corre depois de CriaDadosExemplo e só uma vez: se a primeira unidade de demonstração
    // já existir, não faz nada. Categorias, características e regimes são procurados pelo
    // nome e só se criam os que faltam: serve qualquer base de dados, nova ou já com dados.
    public static async Task CriaDadosDemonstracao(ApplicationDbContext context, string pastaImagens)
    {
        if (await context.Hoteis.AnyAsync(h => h.Nome == "Hotel Baixa Pombalina"))
        {
            return;
        }

        byte[]? Le(string ficheiro)
        {
            var caminho = Path.Combine(pastaImagens, ficheiro);
            return File.Exists(caminho) ? File.ReadAllBytes(caminho) : null;
        }

        // O que já existe na base de dados, para não criar nomes repetidos (índices únicos).
        var categorias = await context.Categorias.ToListAsync();
        var caracteristicas = await context.Caracteristicas.ToListAsync();
        var regimes = await context.RegimesAlimentares.ToListAsync();
        var modos = await context.ModosDisponibilizacao.ToListAsync();

        // Devolvem o registo com esse nome; se não existir, criam-no.
        Categoria Cat(string nome, string descricao, string icone)
        {
            var c = categorias.FirstOrDefault(x => x.Nome == nome);
            if (c is null)
            {
                c = new Categoria { Nome = nome, Descricao = descricao, Icone = icone };
                categorias.Add(c);
                context.Categorias.Add(c);
            }
            return c;
        }
        Caracteristica Car(string nome, string icone)
        {
            var c = caracteristicas.FirstOrDefault(x => x.Nome == nome);
            if (c is null)
            {
                c = new Caracteristica { Nome = nome, Icone = icone };
                caracteristicas.Add(c);
                context.Caracteristicas.Add(c);
            }
            return c;
        }
        RegimeAlimentar Reg(string nome, string descricao, decimal percentagem, string icone)
        {
            var r = regimes.FirstOrDefault(x => x.Nome == nome);
            if (r is null)
            {
                r = new RegimeAlimentar { Nome = nome, Descricao = descricao, Percentagem = percentagem, Icone = icone };
                regimes.Add(r);
                context.RegimesAlimentares.Add(r);
            }
            return r;
        }
        ModoDisponibilizacao Modo(string nome, string descricao)
        {
            var m = modos.FirstOrDefault(x => x.Nome == nome);
            if (m is null)
            {
                m = new ModoDisponibilizacao { Nome = nome, Descricao = descricao };
                modos.Add(m);
                context.ModosDisponibilizacao.Add(m);
            }
            return m;
        }

        var reserva = Modo("Reserva", "Pode ser reservado");
        var listagem = Modo("Listagem", "Aparece na aplicação, mas não pode ser reservado");

        // Regimes: os quatro dos dados de exemplo e um novo.
        var semRefeicoes = Reg("Sem refeições", "Só alojamento", 0, "sem-refeicoes");
        var pequenoAlmoco = Reg("Pequeno-almoço", "Alojamento e pequeno-almoço", 10, "pequeno-almoco");
        var meiaPensao = Reg("Meia pensão", "Pequeno-almoço e jantar", 30, "meia-pensao");
        var pensaoCompleta = Reg("Pensão completa", "Pequeno-almoço, almoço e jantar", 45, "pensao-completa");
        var tudoIncluido = Reg("Tudo incluído", "Todas as refeições, bebidas e snacks ao longo do dia", 65, "pensao-completa");

        // Categorias: as dos dados de exemplo e oito novas.
        var individual = Cat("Quarto Individual", "Quarto para uma pessoa", "quarto-individual");
        var duplo = Cat("Quarto Duplo", "Quarto para duas pessoas", "quarto-duplo");
        var triplo = Cat("Quarto Triplo", "Quarto para três pessoas, com cama de casal e cama individual", "quarto-familiar");
        var familiar = Cat("Quarto Familiar", "Quarto para quatro pessoas, com cama de casal e duas camas de solteiro", "quarto-familiar");
        var suite = Cat("Suite", "Quarto com zona de estar", "suite");
        var suiteJunior = Cat("Suite Júnior", "Quarto amplo com pequena zona de estar", "suite");
        var estudio = Cat("Apartamento T0", "Estúdio: apartamento de uma só divisão, com kitchenette", "apartamento");
        var t1 = Cat("Apartamento T1", "Apartamento com um quarto e kitchenette", "apartamento");
        var t2 = Cat("Apartamento T2", "Apartamento com dois quartos e cozinha", "apartamento");
        var t3 = Cat("Apartamento T3", "Apartamento com três quartos, sala e cozinha", "apartamento");
        var duplex = Cat("Duplex", "Apartamento em dois pisos, com sala em baixo e quartos em cima", "apartamento");
        var penthouse = Cat("Penthouse", "Apartamento no último piso, com terraço privativo", "suite");

        // Características: as dos dados de exemplo e nove novas (todas com ícone próprio).
        var wifi = Car("Wi-Fi", "wifi");
        var ac = Car("Ar condicionado", "ar-condicionado");
        var varanda = Car("Varanda", "varanda");
        var vistaMar = Car("Vista mar", "vista-mar");
        var kitchenette = Car("Kitchenette", "kitchenette");
        var banheira = Car("Banheira", "banheira");
        var lareira = Car("Lareira", "lareira");
        var vistaMontanha = Car("Vista montanha", "vista-montanha");
        var vistaRio = Car("Vista rio", "vista-rio");
        var televisao = Car("Televisão", "televisao");
        var frigorifico = Car("Minibar", "frigorifico");
        var cofre = Car("Cofre", "cofre");
        var estacionamento = Car("Estacionamento", "estacionamento");
        var acessivel = Car("Acessível a mobilidade reduzida", "acessivel");
        var animais = Car("Aceita animais", "animais");

        // As unidades. A imagem de cada uma é a fotografia do seu primeiro alojamento.
        Hotel H(string nome, string tipo, int estrelas, string descricao, string imagem,
                string rua, string codigoPostal, string localidade, string distrito, bool ativo = true) => new()
                {
                    Nome = nome,
                    Tipo = tipo,
                    Estrelas = estrelas,
                    Descricao = descricao,
                    UrlImagem = imagem,
                    Imagem = Le(imagem),
                    Ativo = ativo,
                    morada = new Morada { Rua = rua, CodigoPostal = codigoPostal, Localidade = localidade, Distrito = distrito }
                };

        var baixa = H("Hotel Baixa Pombalina", TipoHotel.Hotel, 4, "Hotel boutique num edifício pombalino recuperado, a dois passos da Praça do Comércio.",
                      "01-quarto-individual-urbano.jpg", "Rua Augusta 120", "1100-053", "Lisboa", "Lisboa");
        var alfama = H("Hotel Alfama Miradouro", TipoHotel.Hotel, 3, "Pequeno hotel familiar no coração de Alfama, com vista sobre o Tejo e os telhados da cidade.",
                       "19-suite-panorama.jpg", "Rua de São Tomé 18", "1100-563", "Lisboa", "Lisboa");
        var nacoes = H("Aparthotel Parque das Nações", TipoHotel.Aparthotel, 4, "Apartamentos modernos junto ao rio, perto do Oceanário e da estação do Oriente.",
                       "25-apartamento-estudio.jpg", "Avenida D. João II 40", "1990-095", "Lisboa", "Lisboa");
        var foz = H("Hotel Foz do Douro", TipoHotel.Hotel, 4, "Hotel junto à praia, onde o Douro encontra o Atlântico, com restaurante de peixe fresco.",
                    "07-quarto-duplo-atlantico.jpg", "Avenida do Brasil 230", "4150-153", "Porto", "Porto");
        var clerigos = H("Aparthotel Clérigos", TipoHotel.Aparthotel, 3, "Apartamentos num prédio histórico, ao pé da Torre dos Clérigos e da Livraria Lello.",
                         "26-apartamento-t1-ribeira.jpg", "Rua das Carmelitas 85", "4050-161", "Porto", "Porto");
        var sintra = H("Hotel Sintra Palácio", TipoHotel.Hotel, 5, "Antigo palacete do século XIX rodeado de jardins, com spa e vista para a serra de Sintra.",
                       "08-quarto-duplo-jardim.jpg", "Rua Visconde de Monserrate 12", "2710-591", "Sintra", "Lisboa");
        var cascais = H("Hotel Cascais Baía", TipoHotel.Hotel, 4, "Hotel virado à baía de Cascais, a poucos minutos a pé da marina e da praia da Rainha.",
                        "02-quarto-individual-costa.jpg", "Avenida Dom Carlos I 9", "2750-310", "Cascais", "Lisboa");
        var lagos = H("Hotel Marina de Lagos", TipoHotel.Hotel, 4, "Hotel com piscina sobre a marina, perto da Ponta da Piedade e das praias de falésia.",
                      "11-quarto-duplo-varanda.jpg", "Avenida dos Descobrimentos 51", "8600-645", "Lagos", "Faro");
        var rocha = H("Aparthotel Praia da Rocha", TipoHotel.Aparthotel, 3, "Apartamentos com varanda sobre o areal da Praia da Rocha, com piscina e bar de praia.",
                      "27-apartamento-t1-varanda.jpg", "Avenida Tomás Cabreira 70", "8500-802", "Portimão", "Faro");
        var tavira = H("Hotel Ria Formosa", TipoHotel.Hotel, 4, "Hotel tranquilo junto à Ria Formosa, com barco para a Ilha de Tavira incluído.",
                       "13-quarto-familiar-beliche.jpg", "Rua Gil Eanes 15", "8800-310", "Tavira", "Faro");
        var albufeira = H("Aparthotel Albufeira Sol", TipoHotel.Aparthotel, 3, "Apartamentos para famílias, com piscina para crianças e a cinco minutos da praia dos Pescadores.",
                          "28-apartamento-t2-familiar.jpg", "Rua Alexandre Herculano 24", "8200-140", "Albufeira", "Faro");
        var coimbra = H("Hotel Universidade Coimbra", TipoHotel.Hotel, 3, "Hotel clássico na Alta de Coimbra, a poucos passos da Universidade e da Biblioteca Joanina.",
                        "06-quarto-individual-classico.jpg", "Rua Larga 8", "3000-370", "Coimbra", "Coimbra");
        var aveiro = H("Hotel Ria de Aveiro", TipoHotel.Hotel, 3, "Hotel de design junto aos canais, com bicicletas gratuitas para passear até à Costa Nova.",
                       "12-quarto-duplo-minimalista.jpg", "Rua João Mendonça 20", "3800-200", "Aveiro", "Aveiro");
        var braga = H("Hotel Sé de Braga", TipoHotel.Hotel, 3, "Hotel no centro histórico, entre a Sé e a Avenida da Liberdade.",
                      "14-quarto-familiar-triplo.jpg", "Rua do Souto 60", "4700-329", "Braga", "Braga");
        var evora = H("Hotel Templo de Évora", TipoHotel.Hotel, 4, "Antigo solar alentejano dentro das muralhas, com pátio interior e piscina.",
                      "05-quarto-individual-patio.jpg", "Largo do Conde de Vila Flor 5", "7000-804", "Évora", "Évora");
        var douro = H("Hotel Douro Vinhateiro", TipoHotel.Hotel, 5, "Quinta vinícola com hotel de charme, sobre os socalcos do Douro, com provas de vinho do Porto.",
                      "10-quarto-duplo-twin-rural.jpg", "Estrada Nacional 222, km 18", "5050-301", "Peso da Régua", "Vila Real");
        var funchal = H("Aparthotel Funchal Atlântico", TipoHotel.Aparthotel, 4, "Apartamentos com vista sobre o Atlântico, no Lido do Funchal, com piscina de água salgada.",
                        "29-apartamento-duplex.jpg", "Estrada Monumental 175", "9000-100", "Funchal", "Madeira");
        var pontaDelgada = H("Hotel Ponta Delgada Lagoas", TipoHotel.Hotel, 4, "Hotel na marginal de Ponta Delgada, com passeios organizados às Sete Cidades e às Furnas.",
                             "16-quarto-familiar-espacoso.jpg", "Avenida Infante Dom Henrique 42", "9500-150", "Ponta Delgada", "Açores");
        var nazare = H("Hotel Nazaré Ondas", TipoHotel.Hotel, 3, "Hotel na marginal da Nazaré. Fechado para remodelação até ao fim de março.",
                       "18-quarto-familiar-mar.jpg", "Avenida da República 30", "2450-101", "Nazaré", "Leiria", ativo: false);
        var geres = H("Hotel Gerês Natura", TipoHotel.Hotel, 3, "Hotel de montanha no Parque Nacional da Peneda-Gerês, com trilhos a partir da porta.",
                      "03-quarto-individual-serra.jpg", "Avenida Manuel Francisco da Costa 11", "4845-067", "Gerês", "Braga");
        context.Hoteis.AddRange(baixa, alfama, nacoes, foz, clerigos, sintra, cascais, lagos, rocha, tavira,
                                albufeira, coimbra, aveiro, braga, evora, douro, funchal, pontaDelgada, nazare, geres);

        // Os alojamentos: um por fotografia de wwwroot/img/seed.
        Alojamento A(string nome, string descricao, Hotel hotel, Categoria categoria, int capacidade, int unidades,
                     decimal preco, string imagem, RegimeAlimentar[] regimesAloj, Caracteristica[] caracteristicasAloj,
                     ModoDisponibilizacao? modo = null, bool ativo = true) => new()
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
                         Ativo = ativo,
                         regimes = regimesAloj.ToList(),
                         caracteristicas = caracteristicasAloj.ToList()
                     };

        context.Alojamentos.AddRange(
            // Quartos individuais
            A("Quarto Individual Urbano", "Quarto individual insonorizado, com secretária, virado para a Rua Augusta.", baixa, individual, 1, 8, 75m,
              "01-quarto-individual-urbano.jpg", [semRefeicoes, pequenoAlmoco], [wifi, ac, televisao, cofre]),
            A("Quarto Individual Costa", "Quarto individual com janela sobre a baía e luz natural todo o dia.", cascais, individual, 1, 6, 85m,
              "02-quarto-individual-costa.jpg", [semRefeicoes, pequenoAlmoco], [wifi, ac, vistaMar, televisao]),
            A("Quarto Individual Serra", "Quarto simples e acolhedor, em madeira, com vista para a serra do Gerês.", geres, individual, 1, 10, 55m,
              "03-quarto-individual-serra.jpg", [pequenoAlmoco, meiaPensao], [wifi, vistaMontanha, estacionamento, animais]),
            A("Quarto Individual Executivo", "Quarto para viagens de negócios, com secretária ampla e cafeteira.", foz, individual, 1, 12, 90m,
              "04-quarto-individual-executivo.jpg", [semRefeicoes, pequenoAlmoco], [wifi, ac, televisao, frigorifico, cofre]),
            A("Quarto Individual Pátio", "Quarto individual térreo, com porta para o pátio interior do solar.", evora, individual, 1, 4, 70m,
              "05-quarto-individual-patio.jpg", [semRefeicoes, pequenoAlmoco], [wifi, ac, acessivel]),
            A("Quarto Individual Clássico", "Quarto individual com mobiliário de época. Em remodelação: volta em breve.", coimbra, individual, 1, 6, 58m,
              "06-quarto-individual-classico.jpg", [semRefeicoes, pequenoAlmoco], [wifi, televisao], ativo: false),

            // Quartos duplos
            A("Quarto Duplo Atlântico", "Quarto duplo com janela panorâmica sobre o mar da Foz.", foz, duplo, 2, 18, 125m,
              "07-quarto-duplo-atlantico.jpg", [semRefeicoes, pequenoAlmoco, meiaPensao], [wifi, ac, vistaMar, televisao, frigorifico]),
            A("Quarto Duplo Jardim", "Quarto duplo com acesso direto aos jardins do palacete.", sintra, duplo, 2, 10, 160m,
              "08-quarto-duplo-jardim.jpg", [pequenoAlmoco, meiaPensao], [wifi, ac, televisao, banheira]),
            A("Quarto Twin Urbano", "Quarto com duas camas individuais, ideal para amigos ou colegas de trabalho.", baixa, duplo, 2, 14, 110m,
              "09-quarto-duplo-twin-urbano.jpg", [semRefeicoes, pequenoAlmoco], [wifi, ac, televisao, cofre]),
            A("Quarto Twin Vinha", "Quarto com duas camas e vista sobre as vinhas e o rio Douro.", douro, duplo, 2, 8, 150m,
              "10-quarto-duplo-twin-rural.jpg", [pequenoAlmoco, meiaPensao, pensaoCompleta], [wifi, vistaRio, vistaMontanha, estacionamento]),
            A("Quarto Duplo com Varanda", "Quarto duplo com varanda privada sobre a marina.", lagos, duplo, 2, 20, 130m,
              "11-quarto-duplo-varanda.jpg", [semRefeicoes, pequenoAlmoco, meiaPensao], [wifi, ac, varanda, vistaMar, televisao]),
            A("Quarto Duplo Minimalista", "Quarto duplo de design, em tons claros, com vista para os canais.", aveiro, duplo, 2, 16, 85m,
              "12-quarto-duplo-minimalista.jpg", [semRefeicoes, pequenoAlmoco], [wifi, ac, televisao, acessivel]),

            // Quartos familiares e triplos
            A("Quarto Familiar com Beliche", "Quarto com cama de casal e beliche, pensado para famílias com crianças.", tavira, familiar, 4, 8, 145m,
              "13-quarto-familiar-beliche.jpg", [pequenoAlmoco, meiaPensao, pensaoCompleta], [wifi, ac, televisao, frigorifico]),
            A("Quarto Triplo", "Quarto com cama de casal e cama individual, perto da Sé.", braga, triplo, 3, 6, 95m,
              "14-quarto-familiar-triplo.jpg", [semRefeicoes, pequenoAlmoco], [wifi, ac, televisao]),
            A("Quartos Comunicantes", "Dois quartos ligados por uma porta interior, para famílias até cinco pessoas.", cascais, familiar, 5, 4, 210m,
              "15-quarto-familiar-comunicante.jpg", [pequenoAlmoco, meiaPensao], [wifi, ac, vistaMar, televisao, banheira]),
            A("Quarto Familiar Espaçoso", "Quarto familiar com zona de estar e sofá-cama.", pontaDelgada, familiar, 4, 10, 150m,
              "16-quarto-familiar-espacoso.jpg", [pequenoAlmoco, meiaPensao, pensaoCompleta], [wifi, ac, vistaMar, televisao, acessivel]),
            A("Quarto Familiar Campo", "Quarto familiar rústico, com vista para o rio Homem.", geres, familiar, 4, 6, 110m,
              "17-quarto-familiar-campo.jpg", [pequenoAlmoco, meiaPensao, pensaoCompleta], [wifi, vistaMontanha, lareira, estacionamento, animais]),
            A("Quarto Familiar Vista Mar", "Quarto familiar com varanda sobre a praia da Nazaré.", nazare, familiar, 4, 8, 140m,
              "18-quarto-familiar-mar.jpg", [pequenoAlmoco, meiaPensao], [wifi, varanda, vistaMar, televisao]),

            // Suites
            A("Suite Panorama", "Suite no último piso, com janelas para o Tejo e o Castelo de São Jorge.", alfama, suite, 2, 2, 220m,
              "19-suite-panorama.jpg", [pequenoAlmoco, meiaPensao], [wifi, ac, vistaRio, televisao, banheira, frigorifico]),
            A("Suite Lareira Douro", "Suite com lareira, sala e varanda sobre os socalcos.", douro, suite, 2, 3, 290m,
              "20-suite-lareira.jpg", [pequenoAlmoco, meiaPensao, pensaoCompleta], [wifi, lareira, varanda, vistaRio, banheira]),
            A("Suite Júnior", "Suite júnior com zona de leitura e vista para os canais.", aveiro, suiteJunior, 2, 6, 140m,
              "21-suite-junior.jpg", [semRefeicoes, pequenoAlmoco, meiaPensao], [wifi, ac, televisao, frigorifico]),
            A("Suite com Terraço", "Suite com terraço privado e espreguiçadeiras sobre a marina.", lagos, suite, 2, 4, 260m,
              "22-suite-terraco.jpg", [pequenoAlmoco, meiaPensao, tudoIncluido], [wifi, ac, varanda, vistaMar, banheira, frigorifico]),
            A("Suite Spa", "Suite com banheira de hidromassagem e acesso ilimitado ao spa.", sintra, suite, 2, 3, 340m,
              "23-suite-spa.jpg", [pequenoAlmoco, meiaPensao, pensaoCompleta, tudoIncluido], [wifi, ac, banheira, televisao, cofre]),
            A("Suite Histórica", "Suite com tetos pintados do século XVIII. Só para visita: não se reserva pela aplicação.", evora, suite, 2, 1, 230m,
              "24-suite-historica.jpg", [pequenoAlmoco, meiaPensao], [wifi, ac, banheira], listagem),

            // Apartamentos
            A("Estúdio Tejo", "Estúdio com kitchenette e janela sobre o rio.", nacoes, estudio, 2, 10, 95m,
              "25-apartamento-estudio.jpg", [semRefeicoes, pequenoAlmoco], [wifi, ac, kitchenette, vistaRio, televisao]),
            A("Apartamento T1 Clérigos", "Apartamento T1 num prédio do século XIX, com tetos altos e soalho original.", clerigos, t1, 3, 8, 115m,
              "26-apartamento-t1-ribeira.jpg", [semRefeicoes, pequenoAlmoco], [wifi, ac, kitchenette, televisao]),
            A("Apartamento T1 com Varanda", "Apartamento T1 com varanda sobre a praia.", rocha, t1, 3, 12, 120m,
              "27-apartamento-t1-varanda.jpg", [semRefeicoes, pequenoAlmoco], [wifi, ac, kitchenette, varanda, vistaMar]),
            A("Apartamento T2 Familiar", "Apartamento T2 com sala, cozinha completa e varanda.", albufeira, t2, 5, 10, 165m,
              "28-apartamento-t2-familiar.jpg", [semRefeicoes, pequenoAlmoco], [wifi, ac, kitchenette, varanda, televisao, estacionamento]),
            A("Apartamento Duplex", "Duplex com sala em baixo, dois quartos em cima e vista de mar.", funchal, duplex, 6, 4, 230m,
              "29-apartamento-duplex.jpg", [semRefeicoes, pequenoAlmoco, meiaPensao], [wifi, ac, kitchenette, vistaMar, televisao]),
            A("Penthouse Tejo", "Penthouse com terraço privativo e jacúzi. Reserva sob consulta: não se reserva pela aplicação.", nacoes, penthouse, 6, 1, 390m,
              "30-apartamento-penthouse.jpg", [semRefeicoes, pequenoAlmoco], [wifi, ac, kitchenette, varanda, vistaRio, banheira], listagem)
        );

        // Campanhas: épocas altas (aumentam o preço) e descontos, no próximo ano.
        // Na mesma unidade os períodos não se sobrepõem (RN51).
        var ano = DateTime.Today.Year + 1;
        Campanha C(Hotel hotel, string nome, string descricao, decimal percentagem, DateTime inicio, DateTime fim) =>
            new() { hotel = hotel, Nome = nome, Descricao = descricao, Percentagem = percentagem, DataInicio = inicio, DataFim = fim };

        context.Campanhas.AddRange(
            C(baixa, "Santos Populares", "Junho em Lisboa", 15, new DateTime(ano, 6, 10), new DateTime(ano, 6, 30)),
            C(baixa, "Inverno em Lisboa", "Desconto na época baixa", -15, new DateTime(ano, 1, 7), new DateTime(ano, 2, 28)),
            C(foz, "São João", "A noite mais longa do Porto", 20, new DateTime(ano, 6, 20), new DateTime(ano, 6, 25)),
            C(sintra, "Natal na Serra", "Mercado de Natal e jantar de consoada", 10, new DateTime(ano, 12, 1), new DateTime(ano, 12, 23)),
            C(lagos, "Época alta de verão", "Julho e agosto", 25, new DateTime(ano, 7, 1), new DateTime(ano, 8, 31)),
            C(lagos, "Outono no Algarve", "Desconto depois do verão", -20, new DateTime(ano, 10, 1), new DateTime(ano, 11, 30)),
            C(rocha, "Verão na Rocha", "Julho e agosto", 30, new DateTime(ano, 7, 1), new DateTime(ano, 8, 31)),
            C(albufeira, "Verão em família", "Julho e agosto", 30, new DateTime(ano, 7, 1), new DateTime(ano, 8, 31)),
            C(albufeira, "Primavera no Algarve", "Desconto na primavera", -10, new DateTime(ano, 3, 15), new DateTime(ano, 5, 31)),
            C(douro, "Vindimas", "Participação nas vindimas e prova de vinhos", 20, new DateTime(ano, 9, 1), new DateTime(ano, 10, 15)),
            C(funchal, "Fim de Ano na Madeira", "Fogo de artifício na baía do Funchal", 35, new DateTime(ano, 12, 27), new DateTime(ano + 1, 1, 2)),
            C(pontaDelgada, "Primavera nos Açores", "Desconto em abril e maio", -12, new DateTime(ano, 4, 1), new DateTime(ano, 5, 31))
        );

        await context.SaveChangesAsync();
    }
}