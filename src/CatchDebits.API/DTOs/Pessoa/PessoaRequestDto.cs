namespace CatchDebits.API.DTOs.Pessoa;

/// <summary>
/// DTO de entrada para criação de uma Pessoa.
/// Recebe apenas os dados que o cliente pode informar — Id é gerado pelo banco.
/// </summary>
public record PessoaRequestDto(
    string Nome,
    int Idade
);