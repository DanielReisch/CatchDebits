using CatchDebits.API.DTOs.Transacao;
using CatchDebits.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatchDebits.API.Controllers;

/// <summary>
/// Controller REST para Transações e Relatório de Totais.
/// A validação da regra de negócio (menor de idade não pode ter Receita)
/// é interceptada automaticamente pelo FluentValidation antes de chegar aqui.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TransacoesController : ControllerBase
{
    private readonly ITransacaoService _service;

    public TransacoesController(ITransacaoService service) => _service = service;

    /// <summary>
    /// GET /api/transacoes — Lista todas as transações com o nome da pessoa vinculada.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TransacaoResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar()
    {
        var transacoes = await _service.ListarTodasAsync();
        return Ok(transacoes);
    }

    /// <summary>
    /// POST /api/transacoes — Cria uma nova transação.
    /// Se menor de 18 + Receita → FluentValidation retorna 400 automaticamente.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TransacaoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] TransacaoRequestDto dto)
    {
        var criada = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(Listar), new { id = criada.Id }, criada);
    }

    /// <summary>
    /// GET /api/transacoes/totais — Relatório financeiro agrupado por pessoa com totais gerais.
    /// </summary>
    [HttpGet("totais")]
    [ProducesResponseType(typeof(RelatorioTotaisDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterTotais()
    {
        var relatorio = await _service.ObterRelatorioTotaisAsync();
        return Ok(relatorio);
    }
}