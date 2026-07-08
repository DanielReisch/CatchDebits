using CatchDebits.API.Data;
using CatchDebits.API.DTOs.Pessoa;
using CatchDebits.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CatchDebits.API.Services;

/// <summary>
/// Implementação da lógica de negócio de Pessoas.
/// O Cascade Delete das transações é tratado automaticamente pelo EF Core
/// conforme configurado no AppDbContext (DeleteBehavior.Cascade).
/// </summary>
public class PessoaService : IPessoaService
{
    private readonly AppDbContext _context;

    public PessoaService(AppDbContext context) => _context = context;

    /// <summary>
    /// Lista todas as pessoas projetando diretamente para DTO.
    /// AsNoTracking() melhora performance: EF Core não aloca
    /// objetos de rastreamento para queries somente leitura.
    /// </summary>
    public async Task<IEnumerable<PessoaResponseDto>> ListarTodosAsync()
    {
        return await _context.Pessoas
            .AsNoTracking()
            .Select(p => new PessoaResponseDto(p.Id, p.Nome, p.Idade))
            .ToListAsync();
    }

    /// <summary>
    /// Cria uma nova pessoa e retorna o DTO com o Id gerado pelo banco.
    /// </summary>
    public async Task<PessoaResponseDto> CriarAsync(PessoaRequestDto dto)
    {
        var pessoa = new Pessoa
        {
            Nome = dto.Nome,
            Idade = dto.Idade
        };

        _context.Pessoas.Add(pessoa);
        await _context.SaveChangesAsync();

        return new PessoaResponseDto(pessoa.Id, pessoa.Nome, pessoa.Idade);
    }

    /// <summary>
    /// Deleta a pessoa pelo Id.
    /// O EF Core com Cascade configurado apaga automaticamente
    /// todas as transações vinculadas antes de apagar a pessoa.
    /// Retorna false se não encontrada — Controller retorna 404.
    /// </summary>
    public async Task<bool> DeletarAsync(int id)
    {
        var pessoa = await _context.Pessoas.FindAsync(id);

        if (pessoa is null) return false;

        _context.Pessoas.Remove(pessoa);
        await _context.SaveChangesAsync();

        return true;
    }
}