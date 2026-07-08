using CatchDebits.API.DTOs.Transacao;

namespace CatchDebits.API.Services;

/// <summary>
/// Interface do serviço de Transações.
/// </summary>
public interface ITransacaoService
{
    Task<IEnumerable<TransacaoResponseDto>> ListarTodasAsync();
    Task<TransacaoResponseDto> CriarAsync(TransacaoRequestDto dto);
    Task<RelatorioTotaisDto> ObterRelatorioTotaisAsync();
}