using CatchDebits.API.DTOs.Transacao;
using FluentValidation;

namespace CatchDebits.API.Validators;

public class TransacaoRequestValidator : AbstractValidator<TransacaoRequestDto>
{
    public TransacaoRequestValidator()
    {
        RuleFor(t => t.Descricao)
            .NotEmpty().WithMessage("A descricao e obrigatoria.")
            .MaximumLength(255).WithMessage("Maximo 255 caracteres.");

        RuleFor(t => t.Valor)
            .GreaterThan(0).WithMessage("O valor deve ser maior que zero.");

        RuleFor(t => t.PessoaId)
            .GreaterThan(0).WithMessage("PessoaId e obrigatorio.");
    }
}
