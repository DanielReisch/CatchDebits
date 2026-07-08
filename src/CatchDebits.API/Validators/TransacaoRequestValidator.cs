using CatchDebits.API.Data;
using CatchDebits.API.DTOs.Transacao;
using CatchDebits.API.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CatchDebits.API.Validators;

/// <summary>
/// Validator responsável por todas as regras de negócio da criação de Transação.
/// Herda de AbstractValidator do FluentValidation — padrão declarativo e testável.
/// É registrado automaticamente no Program.cs via AddValidatorsFromAssemblyContaining.
/// </summary>
public class TransacaoRequestValidator : AbstractValidator<TransacaoRequestDto>
{
    private readonly AppDbContext _context;

    public TransacaoRequestValidator(AppDbContext context)
    {
        _context = context;

        // Regra 1: Descrição obrigatória e com limite de tamanho
        RuleFor(t => t.Descricao)
            .NotEmpty().WithMessage("A descrição é obrigatória.")
            .MaximumLength(255).WithMessage("A descrição deve ter no máximo 255 caracteres.");

        // Regra 2: Valor deve ser positivo
        RuleFor(t => t.Valor)
            .GreaterThan(0).WithMessage("O valor deve ser maior que zero.");

        // Regra 3: PessoaId deve referenciar uma pessoa existente no banco
        RuleFor(t => t.PessoaId)
            .MustAsync(PessoaExisteAsync)
            .WithMessage("A pessoa informada não existe.");

        // REGRA CRÍTICA DE NEGÓCIO:
        // Menores de 18 anos NÃO podem ter transações do tipo Receita.
        // O .When() evita executar esta regra quando o tipo for Despesa,
        // poupando uma query desnecessária ao banco.
        RuleFor(t => t)
            .MustAsync(ValidarTipoParaMenorDeIdadeAsync)
            .WithMessage("Menores de 18 anos não podem registrar Receitas. Apenas Despesas são permitidas.")
            .When(t => t.Tipo == TipoTransacao.Receita);
    }

    /// <summary>
    /// Verifica se a Pessoa existe no banco antes de criar a transação.
    /// Evita erros de FK violation e retorna mensagem amigável ao cliente.
    /// </summary>
    private async Task<bool> PessoaExisteAsync(int pessoaId, CancellationToken ct)
    {
        return await _context.Pessoas.AnyAsync(p => p.Id == pessoaId, ct);
    }

    /// <summary>
    /// REGRA CORE: busca a pessoa e verifica se é menor de 18
    /// tentando cadastrar uma Receita.
    /// Retorna false (inválido) se idade menor de 18 + tipo Receita.
    /// Retorna true (válido) em qualquer outro cenário.
    /// </summary>
    private async Task<bool> ValidarTipoParaMenorDeIdadeAsync(
        TransacaoRequestDto dto,
        CancellationToken ct)
    {
        var pessoa = await _context.Pessoas
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == dto.PessoaId, ct);

        // Se a pessoa não existe, a regra PessoaExisteAsync já trata o erro
        if (pessoa is null) return true;

        // Bloqueia: menor de 18 tentando cadastrar Receita
        return !(pessoa.Idade < 18 && dto.Tipo == TipoTransacao.Receita);
    }
}