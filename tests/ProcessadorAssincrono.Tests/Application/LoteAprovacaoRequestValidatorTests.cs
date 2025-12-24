using ProcessadorAssincrono.Application.Validators;
using ProcessadorAssincrono.Domain.DTOs;
using Shouldly;

namespace ProcessadorAssincrono.Tests.Application.Validators
{
    public class LoteAprovacaoRequestValidatorTests
    {
        [Fact(DisplayName = "Validação deve falhar quando Aprovacoes estiver vazia")]
        public void Validate_ShouldFail_WhenAprovacoesIsEmpty()
        {
            // Arrange
            var validator = new LoteAprovacaoRequestValidator();
            var request = new LoteAprovacaoRequest(new List<Guid>());

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(e => e.PropertyName == "Aprovacoes" && e.ErrorMessage.Contains("válidos"));
        }

        [Fact(DisplayName = "Validação deve falhar quando Aprovacoes contiver Guid.Empty")]
        public void Validate_ShouldFail_WhenContainsEmptyGuid()
        {
            // Arrange
            var validator = new LoteAprovacaoRequestValidator();
            var list = new List<Guid> { Guid.NewGuid(), Guid.Empty, Guid.NewGuid() };
            var request = new LoteAprovacaoRequest(list);

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(e => e.PropertyName == "Aprovacoes" && e.ErrorMessage.Contains("válidos"));
        }

        [Fact(DisplayName = "Validação deve falhar quando Aprovacoes contiver GUIDs duplicados")]
        public void Validate_ShouldFail_WhenContainsDuplicateGuids()
        {
            // Arrange
            var validator = new LoteAprovacaoRequestValidator();
            var duplicate = Guid.NewGuid();
            var list = new List<Guid> { duplicate, Guid.NewGuid(), duplicate };
            var request = new LoteAprovacaoRequest(list);

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(e => e.PropertyName == "Aprovacoes" && e.ErrorMessage.Contains("duplicados"));
        }

        [Fact(DisplayName = "Validação deve passar quando Aprovacoes contiver GUIDs válidos e únicos")]
        public void Validate_ShouldPass_WhenAllGuidsValidAndUnique()
        {
            // Arrange
            var validator = new LoteAprovacaoRequestValidator();
            var list = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var request = new LoteAprovacaoRequest(list);

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.ShouldBeTrue();
            result.Errors.ShouldBeEmpty();
        }
    }
}