using FluentValidation.TestHelper;
using ProcessadorAssincrono.Application.Validators;
using ProcessadorAssincrono.Domain.DTOs;

namespace ProcessadorAssincrono.Tests.Application
{
    public class AprovacaoRequestValidatorTests
    {
        private readonly AprovacaoRequestValidator _validator;

        public AprovacaoRequestValidatorTests()
        {
            _validator = new AprovacaoRequestValidator();
        }

        [Fact]
        public void Deve_Retornar_Erro_Quando_Pep_Estiver_Vazio()
        {
            // Arrange
            var request = new AprovacaoRequest(
                "",
                "Comentário válido",
                DateTime.Now
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Projeto)
                  .WithErrorMessage("O campo Projeto é obrigatório.");
        }

        [Fact]
        public void Deve_Retornar_Erro_Quando_Comentarios_Excederem_500_Caracteres()
        {
            // Arrange
            var request = new AprovacaoRequest(
                "12345",
                new string('a', 501),
                DateTime.Now
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ComentariosAdicionais)
                  .WithErrorMessage("Comentários adicionais não podem exceder 500 caracteres.");
        }

        [Fact]
        public void Deve_Retornar_Erro_Quando_DataAprovacao_Estiver_Vazia()
        {
            // Arrange
            var request = new AprovacaoRequest(
                "12345",
                "Comentário válido",
                default // DateTime.MinValue
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.DataAprovacao)
                  .WithErrorMessage("O campo Data de Aprovação é obrigatório.");
        }

        [Fact]
        public void Deve_Passar_Quando_Dados_Forem_Validos()
        {
            // Arrange
            var request = new AprovacaoRequest(
                "12345",
                "Comentário válido",
                DateTime.Now
            );

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
