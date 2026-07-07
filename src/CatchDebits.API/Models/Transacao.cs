namespace CatchDebits.API.Models;

/// <summary>
/// Enum que representa os tipos possíveis de transação financeira.
/// Usar enum no lugar de string previne erros de digitação
/// e é armazenado como INTEGER no banco, economizando espaço.
/// Valores explícitos garantem estabilidade se o enum for reordenado.
/// </summary>
public enum TipoTransacao
{
    Despesa = 0,
    Receita = 1
}

/// <summary>
/// Entidade que representa uma movimentação financeira.
/// É o lado "muitos" do relacionamento com Pessoa.
/// </summary>
public class Transacao
{
    /// <summary>Chave primária — AUTOINCREMENT no SQLite.</summary>
    public int Id { get; set; }

    /// <summary>
    /// Texto descritivo da transação.
    /// Exemplos: "Salário mensal", "Conta de luz", "Freelance React".
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Valor monetário em reais. Sempre positivo.
    /// O campo Tipo define se é entrada (Receita) ou saída (Despesa).
    /// Tipo decimal(18,2): suporta até 9 quatrilhões com 2 casas decimais.
    /// </summary>
    public decimal Valor { get; set; }

    /// <summary>
    /// Tipo da transação: Despesa (0) ou Receita (1).
    /// REGRA: Se PessoaId referenciar alguém com Idade menor de 18,
    /// este campo DEVE ser Despesa — validado no FluentValidation.
    /// </summary>
    public TipoTransacao Tipo { get; set; }

    /// <summary>
    /// Chave estrangeira para a tabela Pessoas.
    /// Campo obrigatório — toda transação precisa de um dono.
    /// </summary>
    public int PessoaId { get; set; }

    /// <summary>
    /// Propriedade de navegação para a entidade Pessoa relacionada.
    /// O "null!" diz ao compilador que este campo será preenchido
    /// pelo EF Core antes de uso (evita warning de nullable).
    /// </summary>
    public Pessoa Pessoa { get; set; } = null!;
}