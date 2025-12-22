using FluentValidation;
using ProcessadorAssincrono.Domain.DTOs;

namespace ProcessadorAssincrono.Application.Validators
{
    public class LoteAprovacaoRequestValidator : AbstractValidator<LoteAprovacaoRequest>
    {
        public LoteAprovacaoRequestValidator()
        {
            RuleFor(x => x.Aprovacoes)
                .NotNull().WithMessage("A lista de aprovações não pode ser nula.")
                .NotEmpty().WithMessage("É necessário informar pelo menos uma aprovação.")
                .Must(list => list.All(id => id != Guid.Empty))
                .WithMessage("Todos os GUIDs devem ser válidos (não vazios).")
                .Must(list => list.Distinct().Count() == list.Count)
                .WithMessage("A lista não pode conter GUIDs duplicados.");
        }
    }
}
