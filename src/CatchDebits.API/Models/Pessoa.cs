namespace CatchDebits.API.Models;

/// <summary>
/// Entidade que representa uma pessoa no sistema.
/// É o lado "um" do relacionamento 1:N com a entidade Transacao.
/// </summary>
public class Pessoa
{
    /// <summary>
    /// Chave primária. O EF Core reconhece "Id" automaticamente
    /// e configura como AUTOINCREMENT no SQLite.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome completo da pessoa.
    /// Tamanho máximo de 150 caracteres definido no DbContext via Fluent API.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Idade em anos completos.
    /// Campo crítico: determina se a pessoa pode registrar Receitas (mínimo 18 anos).
    /// </summary>
    public int Idade { get; set; }

    /// <summary>
    /// Coleção de navegação — representa o lado "muitos" do relacionamento.
    /// Inicializada como lista vazia para evitar NullReferenceException.
    /// O EF Core popula esta coleção automaticamente ao usar .Include().
    /// </summary>
    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
}