using CatchDebits.API.Models;

namespace CatchDebits.API.DTOs.Transacao;

/// <summary>
/// DTO de saída para exibição de uma Transação.
/// Inclui NomePessoa e TipoDescricao para facilitar o front-end
/// sem ele precisar converter o enum manualmente.
/// </summary>
public record TransacaoResponseDto(
    int Id,
    string Descricao,
    decimal Valor,
    TipoTransacao Tipo,
    string TipoDescricao,
    int PessoaId,
    string NomePessoa
);