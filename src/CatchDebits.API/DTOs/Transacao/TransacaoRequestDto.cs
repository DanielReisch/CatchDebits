using CatchDebits.API.Models;

namespace CatchDebits.API.DTOs.Transacao;

/// <summary>
/// DTO de entrada para criação de uma Transação.
/// O campo Tipo recebe 0 (Despesa) ou 1 (Receita).
/// A validação da regra de idade é feita no TransacaoRequestValidator.
/// </summary>
public record TransacaoRequestDto(
    string Descricao,
    decimal Valor,
    TipoTransacao Tipo,
    int PessoaId
);