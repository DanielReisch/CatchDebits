using CatchDebits.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CatchDebits.API.Data;

/// <summary>
/// Contexto principal do Entity Framework Core.
/// 
/// Responsabilidades:
/// 1. Expor as tabelas como DbSets (propriedades consultáveis via LINQ)
/// 2. Configurar o esquema do banco via Fluent API (OnModelCreating)
/// 3. Ser o ponto de entrada para todas as operações de banco de dados
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Construtor que recebe as opções via Injeção de Dependência.
    /// As opções (qual banco, connection string) são configuradas no Program.cs.
    /// Esse padrão permite trocar SQLite por SQL Server sem alterar esta classe.
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Cada DbSet representa uma tabela no banco de dados
    public DbSet<Pessoa> Pessoas { get; set; }
    public DbSet<Transacao> Transacoes { get; set; }

    /// <summary>
    /// Configuração do modelo via Fluent API.
    /// Executado uma vez na inicialização para configurar o esquema do banco.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ==========================================
        // CONFIGURAÇÃO: Pessoa
        // ==========================================
        modelBuilder.Entity<Pessoa>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Nome)
                  .IsRequired()
                  .HasMaxLength(150);

            entity.Property(p => p.Idade)
                  .IsRequired();
        });

        // ==========================================
        // CONFIGURAÇÃO: Transacao + Relacionamento
        // ==========================================
        modelBuilder.Entity<Transacao>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Descricao)
                  .IsRequired()
                  .HasMaxLength(255);

            // decimal(18,2): necessário declarar explicitamente para o SQLite
            // não perder as casas decimais
            entity.Property(t => t.Valor)
                  .HasColumnType("decimal(18,2)")
                  .IsRequired();

            entity.Property(t => t.Tipo)
                  .IsRequired();

            // ==========================================
            // RELACIONAMENTO 1:N COM CASCADE DELETE
            // ==========================================
            // DeleteBehavior.Cascade significa:
            // Ao executar: DELETE FROM Pessoas WHERE Id = X
            // O banco automaticamente executa: DELETE FROM Transacoes WHERE PessoaId = X
            //
            // Implementa a regra de negócio de deleção em cascata
            // diretamente no banco, sem código C# extra.
            entity.HasOne(t => t.Pessoa)
                  .WithMany(p => p.Transacoes)
                  .HasForeignKey(t => t.PessoaId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);
    }
}