namespace CatchDebits.API.DTOs.Pessoa;

/// <summary>
/// DTO de saída retornado ao cliente após criar ou listar Pessoas.
/// Separado do RequestDto para controlar exatamente o que é exposto na API.
/// </summary>
public record PessoaResponseDto(
    int Id,
    string Nome,
    int Idade
);