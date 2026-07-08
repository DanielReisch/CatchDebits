using CatchDebits.API.Data;
using CatchDebits.API.DTOs.Transacao;
using CatchDebits.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CatchDebits.API.Services;

public class TransacaoService : ITransacaoService
{
    private readonly AppDbContext _context;

    public TransacaoService(AppDbContext context) => _context = context;

    /// <summary>
    /// Lista todas as transações com o nome da pessoa via Include (JOIN automático).
    /// </summary>
    public async Task<IEnumerable<TransacaoResponseDto>> ListarTodasAsync()
    {
        return await _context.Transacoes
            .AsNoTracking()
            .Include(t => t.Pessoa)
            .Select(t => new TransacaoResponseDto(
                t.Id,
                t.Descricao,
                t.Valor,
                t.Tipo,
                t.Tipo.ToString(), // Converte enum para "Receita" ou "Despesa"
                t.PessoaId,
                t.Pessoa.Nome
            ))
            .ToListAsync();
    }

    /// <summary>
    /// Cria uma nova transação.
    /// A validação da regra de idade já foi feita pelo FluentValidation
    /// antes de chegar neste método — aqui só persistimos.
    /// </summary>
    public async Task<TransacaoResponseDto> CriarAsync(TransacaoRequestDto dto)
    {
        var transacao = new Transacao
        {
            Descricao = dto.Descricao,
            Valor = dto.Valor,
            Tipo = dto.Tipo,
            PessoaId = dto.PessoaId
        };

        _context.Transacoes.Add(transacao);
        await _context.SaveChangesAsync();

        // Recarrega a pessoa para incluir o nome no DTO de resposta
        var pessoa = await _context.Pessoas.FindAsync(dto.PessoaId);

        return new TransacaoResponseDto(
            transacao.Id,
            transacao.Descricao,
            transacao.Valor,
            transacao.Tipo,
            transacao.Tipo.ToString(),
            transacao.PessoaId,
            pessoa!.Nome
        );
    }

    /// <summary>
    /// Gera o relatório de totais por pessoa com totais gerais acumulados.
    ///
    /// Estratégia:
    /// - Uma única query traz pessoas + transações (evita o problema N+1)
    /// - O agrupamento e cálculo acontecem em memória via LINQ to Objects
    /// - Pessoas sem transações aparecem com saldo zerado naturalmente
    /// </summary>
    public async Task<RelatorioTotaisDto> ObterRelatorioTotaisAsync()
    {
        var pessoas = await _context.Pessoas
            .AsNoTracking()
            .Include(p => p.Transacoes)
            .ToListAsync();

        var totaisPorPessoa = pessoas.Select(p =>
        {
            var totalReceitas = p.Transacoes
                .Where(t => t.Tipo == TipoTransacao.Receita)
                .Sum(t => t.Valor);

            var totalDespesas = p.Transacoes
                .Where(t => t.Tipo == TipoTransacao.Despesa)
                .Sum(t => t.Valor);

            return new TotaisPessoaDto(
                p.Id,
                p.Nome,
                p.Idade,
                totalReceitas,
                totalDespesas,
                SaldoLiquido: totalReceitas - totalDespesas
            );
        }).ToList();

        var totalReceitasGeral = totaisPorPessoa.Sum(t => t.TotalReceitas);
        var totalDespesasGeral = totaisPorPessoa.Sum(t => t.TotalDespesas);

        return new RelatorioTotaisDto(
            totaisPorPessoa,
            totalReceitasGeral,
            totalDespesasGeral,
            SaldoLiquidoGeral: totalReceitasGeral - totalDespesasGeral
        );
    }
}