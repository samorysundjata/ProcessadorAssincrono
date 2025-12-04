using FluentValidation;
using ProcessadorAssincrono.Domain.DTOs;

namespace ProcessadorAssincrono.Application.Validators
{
    public class AprovacaoRequestValidator : AbstractValidator<AprovacaoRequest>
    {
        public AprovacaoRequestValidator()
        {
            RuleFor(x => x.Pep).NotEmpty().WithMessage("O campo PEP é obrigatório.");
            RuleFor(x => x.ComentariosAdicionais).MaximumLength(500)
                .WithMessage("Comentários adicionais não podem exceder 500 caracteres.");
            RuleFor(x => x.DataAprovacao).NotEmpty().WithMessage("O campo Data de Aprovação é obrigatório.");
        }
    }
}
