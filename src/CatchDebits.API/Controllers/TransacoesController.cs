using CatchDebits.API.Data;
using CatchDebits.API.DTOs.Transacao;
using CatchDebits.API.Models;
using CatchDebits.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatchDebits.API.Controllers;

/// <summary>
/// Controller REST para Transações e Relatório de Totais.
/// A regra de negócio crítica (menor de 18 não pode ter Receita)
/// é validada aqui de forma assíncrona antes de chamar o Service.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TransacoesController : ControllerBase
{
    private readonly ITransacaoService _service;
    private readonly AppDbContext _context;

    public TransacoesController(ITransacaoService service, AppDbContext context)
    {
        _service = service;
        _context = context;
    }

    /// <summary>
    /// GET /api/transacoes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TransacaoResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar()
    {
        var transacoes = await _service.ListarTodasAsync();
        return Ok(transacoes);
    }

    /// <summary>
    /// POST /api/transacoes
    /// Valida a regra de negócio: menor de 18 não pode ter Receita.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TransacaoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Criar([FromBody] TransacaoRequestDto dto)
    {
        // Verifica se a pessoa existe
        var pessoa = await _context.Pessoas
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == dto.PessoaId);

        if (pessoa is null)
            return NotFound(new { mensagem = $"Pessoa com Id {dto.PessoaId} não encontrada." });

        // REGRA CRÍTICA: menor de 18 não pode ter Receita
        if (pessoa.Idade < 18 && dto.Tipo == TipoTransacao.Receita)
            return BadRequest(new { mensagem = "Menores de 18 anos não podem registrar Receitas. Apenas Despesas são permitidas." });

        var criada = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(Listar), new { id = criada.Id }, criada);
    }

    /// <summary>
    /// GET /api/transacoes/totais
    /// </summary>
    [HttpGet("totais")]
    [ProducesResponseType(typeof(RelatorioTotaisDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterTotais()
    {
        var relatorio = await _service.ObterRelatorioTotaisAsync();
        return Ok(relatorio);
    }
}