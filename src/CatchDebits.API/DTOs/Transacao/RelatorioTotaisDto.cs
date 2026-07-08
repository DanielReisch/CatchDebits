namespace CatchDebits.API.DTOs.Transacao;

/// <summary>
/// DTO de saída para o relatório de totais de uma pessoa específica.
/// SaldoLiquido = TotalReceitas - TotalDespesas.
/// </summary>
public record TotaisPessoaDto(
    int PessoaId,
    string NomePessoa,
    int IdadePessoa,
    decimal TotalReceitas,
    decimal TotalDespesas,
    decimal SaldoLiquido
);

/// <summary>
/// DTO de saída para o relatório completo.
/// Contém a lista por pessoa + os totais gerais acumulados de todas elas.
/// </summary>
public record RelatorioTotaisDto(
    IEnumerable<TotaisPessoaDto> TotaisPorPessoa,
    decimal TotalReceitasGeral,
    decimal TotalDespesasGeral,
    decimal SaldoLiquidoGeral
);