using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
// O namespace onde estão as nossas entidades: Hotel, Alojamento, Reserva, ...
using GestaoHoteis.Entities;

namespace GestaoHoteis.Data;

// O contexto representa a base de dados no código C#.
// Herda de IdentityDbContext, por isso já traz as tabelas dos utilizadores e dos roles.
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    // Registo das nossas entidades no contexto. Cada DbSet passa a ser uma tabela
    // da base de dados, com o nome da propriedade: Moradas, Hoteis, Categorias, ...
    // É também por aqui que se leem e gravam os dados, por exemplo context.Hoteis.
    public DbSet<Morada> Moradas { get; set; }
    public DbSet<Hotel> Hoteis { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Caracteristica> Caracteristicas { get; set; }
    public DbSet<ModoDisponibilizacao> ModosDisponibilizacao { get; set; }
    public DbSet<RegimeAlimentar> RegimesAlimentares { get; set; }
    public DbSet<Alojamento> Alojamentos { get; set; }
    public DbSet<Campanha> Campanhas { get; set; }
    public DbSet<Reserva> Reservas { get; set; }
    public DbSet<LinhaReserva> LinhasReserva { get; set; }
    public DbSet<Pagamento> Pagamentos { get; set; }
    public DbSet<Fatura> Faturas { get; set; }

    // As relações que as convenções não resolvem sozinhas.
    // OnModelCreating é o método onde se configura o modelo por código (Fluent API).
    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Obrigatório e sempre em primeiro lugar: configura as tabelas do Identity.
        base.OnModelCreating(builder);

        // Relações 1-1 com Morada. Por convenção, uma chave estrangeira sozinha dá uma relação 1-N
        // (a mesma morada podia servir várias unidades). WithOne() diz que é 1-1: o EF Core
        // cria um índice único sobre a chave estrangeira, e cada morada só serve uma vez.
        builder.Entity<Hotel>()
            .HasOne(h => h.morada).WithOne()
            .HasForeignKey<Hotel>(h => h.MoradaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Reserva>()
            .HasOne(r => r.moradaFaturacao).WithOne()
            .HasForeignKey<Reserva>(r => r.MoradaFaturacaoId)
            .OnDelete(DeleteBehavior.Restrict);

        // A morada do utilizador também é 1-1 (alínea 3.d). É opcional: MoradaId é int?.
        builder.Entity<ApplicationUser>()
            .HasOne(u => u.morada).WithOne()
            .HasForeignKey<ApplicationUser>(u => u.MoradaId);

        // Apagar em cascata não: uma relação obrigatória, por convenção, apaga os dependentes
        // (apagar uma categoria apagaria os seus alojamentos). Restrict faz o contrário:
        // a base de dados recusa apagar enquanto houver dependentes.
        // Sem estas linhas, o SQL Server recusa criar a base de dados: de Hotel chegava-se
        // a LinhasReserva por dois caminhos em cascata (pelas reservas e pelos alojamentos).
        builder.Entity<Alojamento>()
            .HasOne(a => a.hotel).WithMany(h => h.alojamentos)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Alojamento>()
            .HasOne(a => a.categoria).WithMany(c => c.alojamentos)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Alojamento>()
            .HasOne(a => a.modoDisponibilizacao).WithMany(m => m.alojamentos)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Reserva>()
            .HasOne(r => r.cliente).WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Reserva>()
            .HasOne(r => r.hotel).WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<LinhaReserva>()
            .HasOne(l => l.alojamento).WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<LinhaReserva>()
            .HasOne(l => l.regimeAlimentar).WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        // Regras da base de dados (Etapa 3). São a última garantia: valem para qualquer
        // código que grave nestas tabelas (as páginas, a API, um script).

        // Nomes únicos: um índice único não deixa gravar dois registos com o mesmo nome.
        // O SQL Server não distingue maiúsculas: "Suite" e "suite" contam como iguais.
        builder.Entity<Hotel>().HasIndex(h => h.Nome).IsUnique();
        builder.Entity<Categoria>().HasIndex(c => c.Nome).IsUnique();
        builder.Entity<Caracteristica>().HasIndex(c => c.Nome).IsUnique();
        builder.Entity<ModoDisponibilizacao>().HasIndex(m => m.Nome).IsUnique();
        builder.Entity<RegimeAlimentar>().HasIndex(r => r.Nome).IsUnique();
        // Na mesma unidade não há dois alojamentos com o mesmo nome (índice sobre duas colunas).
        builder.Entity<Alojamento>().HasIndex(a => new { a.HotelId, a.Nome }).IsUnique();
        // As faturas têm numeração sequencial: o número não se repete.
        builder.Entity<Fatura>().HasIndex(f => f.Numero).IsUnique();

        // Restrições CHECK: a base de dados recusa linhas que não cumpram estas condições.
        builder.Entity<Hotel>().ToTable(t =>
        {
            t.HasCheckConstraint("CK_Hoteis_Estrelas", "[Estrelas] BETWEEN 1 AND 5");
            t.HasCheckConstraint("CK_Hoteis_Tipo", "[Tipo] IN (N'Hotel', N'Aparthotel')");
        });

        builder.Entity<Alojamento>().ToTable(t =>
        {
            t.HasCheckConstraint("CK_Alojamentos_Capacidade", "[Capacidade] > 0");
            t.HasCheckConstraint("CK_Alojamentos_NumUnidades", "[NumUnidades] > 0");
            t.HasCheckConstraint("CK_Alojamentos_PrecoNoite", "[PrecoNoite] > 0");
        });

        builder.Entity<RegimeAlimentar>().ToTable(t =>
            t.HasCheckConstraint("CK_RegimesAlimentares_Percentagem", "[Percentagem] >= 0"));

        builder.Entity<Campanha>().ToTable(t =>
        {
            t.HasCheckConstraint("CK_Campanhas_Percentagem", "[Percentagem] > -100 AND [Percentagem] <> 0");
            t.HasCheckConstraint("CK_Campanhas_Datas", "[DataFim] >= [DataInicio]");
        });

        builder.Entity<Reserva>().ToTable(t =>
            t.HasCheckConstraint("CK_Reservas_Datas", "[DataSaida] > [DataEntrada]"));

        builder.Entity<LinhaReserva>().ToTable(t =>
        {
            t.HasCheckConstraint("CK_LinhasReserva_Quantidade", "[Quantidade] > 0");
            t.HasCheckConstraint("CK_LinhasReserva_NumHospedes", "[NumHospedes] > 0");
        });
    }
}