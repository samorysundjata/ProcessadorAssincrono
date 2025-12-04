using Microsoft.Extensions.Logging;
using Moq;
using ProcessadorAssincrono.Domain.Entities;
using ProcessadorAssincrono.Infrastructure.Interfaces;
using ProcessadorAssincrono.Infrastructure.Services;
using Shouldly;

namespace ProcessadorAssincrono.Tests.Application
{
    public class AprovacaoServiceTests
    {
        private static void VerifyLog(Mock<ILogger<AprovacaoService>> mockLogger, LogLevel level, string expectedMessage)
        {
            mockLogger.Verify(l => l.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedMessage)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        [Fact(DisplayName = "AprovarAsync deve chamar InserirAsync e CommitAsync e logar sucesso")]
        public async Task AprovarAsync_ShouldCallInserirAndCommit_WhenSuccess()
        {
            // Arrange
            var mockUow = new Mock<IUnitOfWork>();
            var mockRepo = new Mock<IAprovacaoRepository>();
            mockUow.Setup(u => u.Aprovacoes).Returns(mockRepo.Object);

            var mockLogger = new Mock<ILogger<AprovacaoService>>();

            var service = new AprovacaoService(mockUow.Object, mockLogger.Object);
            var id = Guid.NewGuid();

            // Act
            await service.AprovarAsync(id, "PEP123", "Teste", DateTime.Now);

            // Assert
            mockRepo.Verify(r => r.InserirAsync(It.Is<Aprovacao>(a => a.Id == id)), Times.Once);
            mockUow.Verify(u => u.CommitAsync(), Times.Once);
            VerifyLog(mockLogger, LogLevel.Information, "Aprovação");
        }

        [Fact(DisplayName = "AprovarAsync deve chamar RollbackAsync e logar erro quando ocorrer exceção")]
        public async Task AprovarAsync_ShouldRollbackAndLogError_WhenExceptionOccurs()
        {
            // Arrange
            var mockUow = new Mock<IUnitOfWork>();
            var mockRepo = new Mock<IAprovacaoRepository>();
            mockRepo.Setup(r => r.InserirAsync(It.IsAny<Aprovacao>())).ThrowsAsync(new Exception("Erro simulado"));
            mockUow.Setup(u => u.Aprovacoes).Returns(mockRepo.Object);

            var mockLogger = new Mock<ILogger<AprovacaoService>>();

            var service = new AprovacaoService(mockUow.Object, mockLogger.Object);

            // Act & Assert
            var ex = await Should.ThrowAsync<Exception>(() => service.AprovarAsync(Guid.NewGuid(), "PEP123", "Teste", DateTime.Now));
            ex.Message.ShouldContain("Erro simulado");
            mockUow.Verify(u => u.RollbackAsync(), Times.Once);
            VerifyLog(mockLogger, LogLevel.Error, "Erro ao aprovar");
        }

        [Fact(DisplayName = "InserirAsync deve lançar exceção se aprovação for nula e logar warning")]
        public async Task InserirAsync_ShouldThrow_WhenAprovacaoIsNull()
        {
            // Arrange
            var mockUow = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger<AprovacaoService>>();

            var service = new AprovacaoService(mockUow.Object, mockLogger.Object);

            // Act & Assert
            var ex = await Should.ThrowAsync<ArgumentNullException>(() => service.InserirAsync(null));
            ex.Message.ShouldContain("Aprovação não pode ser nula");
            VerifyLog(mockLogger, LogLevel.Warning, "Tentativa de inserir uma aprovação nula");
        }

        [Fact(DisplayName = "InserirAsync deve chamar InserirAsync e CommitAsync e logar sucesso")]
        public async Task InserirAsync_ShouldCallInserirAndCommit_WhenSuccess()
        {
            // Arrange
            var mockUow = new Mock<IUnitOfWork>();
            var mockRepo = new Mock<IAprovacaoRepository>();
            mockUow.Setup(u => u.Aprovacoes).Returns(mockRepo.Object);

            var mockLogger = new Mock<ILogger<AprovacaoService>>();

            var service = new AprovacaoService(mockUow.Object, mockLogger.Object);
            var aprovacao = new Aprovacao { Id = Guid.NewGuid(), Pep = "PEP123" };

            // Act
            await service.InserirAsync(aprovacao);

            // Assert
            mockRepo.Verify(r => r.InserirAsync(aprovacao), Times.Once);
            mockUow.Verify(u => u.CommitAsync(), Times.Once);
            VerifyLog(mockLogger, LogLevel.Information, "Aprovação");
        }

        [Fact(DisplayName = "InserirAsync deve chamar RollbackAsync e logar erro quando ocorrer exceção")]
        public async Task InserirAsync_ShouldRollbackAndLogError_WhenExceptionOccurs()
        {
            // Arrange
            var mockUow = new Mock<IUnitOfWork>();
            var mockRepo = new Mock<IAprovacaoRepository>();
            mockRepo.Setup(r => r.InserirAsync(It.IsAny<Aprovacao>())).ThrowsAsync(new Exception("Erro simulado"));
            mockUow.Setup(u => u.Aprovacoes).Returns(mockRepo.Object);

            var mockLogger = new Mock<ILogger<AprovacaoService>>();

            var service = new AprovacaoService(mockUow.Object, mockLogger.Object);
            var aprovacao = new Aprovacao { Id = Guid.NewGuid(), Pep = "PEP123" };

            // Act & Assert
            var ex = await Should.ThrowAsync<Exception>(() => service.InserirAsync(aprovacao));
            ex.Message.ShouldContain("Erro simulado");
            mockUow.Verify(u => u.RollbackAsync(), Times.Once);
            VerifyLog(mockLogger, LogLevel.Error, "Erro ao inserir aprovação");
        }
    }
}
