using CatchDebits.API.DTOs.Pessoa;

namespace CatchDebits.API.Services;

/// <summary>
/// Interface do serviço de Pessoas.
/// Programar para interfaces é o D do SOLID (Dependency Inversion):
/// permite trocar a implementação sem alterar quem usa o serviço.
/// </summary>
public interface IPessoaService
{
    Task<IEnumerable<PessoaResponseDto>> ListarTodosAsync();
    Task<PessoaResponseDto> CriarAsync(PessoaRequestDto dto);
    Task<bool> DeletarAsync(int id);
}