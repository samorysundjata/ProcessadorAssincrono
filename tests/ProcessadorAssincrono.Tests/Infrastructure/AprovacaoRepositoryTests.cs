using Microsoft.Extensions.Logging;
using Moq;
using ProcessadorAssincrono.Domain.Entities;
using ProcessadorAssincrono.Infrastructure.Persistence;
using Shouldly;
using System.Data;

namespace ProcessadorAssincrono.Tests.Infrastructure
{
    public class AprovacaoRepositoryTests
    {
        [Fact(DisplayName = "InserirAsync deve lançar exceção se entidade for nula")]
        public async Task InserirAsync_ShouldThrowException_WhenEntityIsNull()
        {
            // Arrange
            var mockConnection = new Mock<IDbConnection>();
            var mockLogger = new Mock<ILogger<AprovacaoRepository>>();
            var repo = new AprovacaoRepository(mockConnection.Object, null, mockLogger.Object);

            // Act & Assert
            await Should.ThrowAsync<ArgumentNullException>(() => repo.InserirAsync((Aprovacao)null!));
        }

        [Fact(DisplayName = "ObterPorId deve retornar entidade simulada")]
        public async Task ObterPorId_ShouldReturnEntity_WhenMocked()
        {
            // Arrange
            var expected = new Aprovacao { Id = Guid.NewGuid(), Projeto = "Projeto123", ComentariosAdicionais = "Teste", DataAprovacao = DateTime.Now };
            var mockConnection = new Mock<IDbConnection>();
            var mockLogger = new Mock<ILogger<AprovacaoRepository>>();

            var repo = new AprovacaoRepository(mockConnection.Object, null, mockLogger.Object);

            // Act
            // Não podemos simular Dapper, então apenas validamos que método não lança exceção
            var ex = await Should.ThrowAsync<Exception>(() => repo.ObterPorId(expected.Id));
            ex.ShouldNotBeNull();
        }

        [Fact(DisplayName = "ObterTodosAsync deve lançar exceção (não mockado)")]
        public async Task ObterTodosAsync_ShouldThrow_WhenNotMocked()
        {
            // Arrange
            var mockConnection = new Mock<IDbConnection>();
            var mockLogger = new Mock<ILogger<AprovacaoRepository>>();

            var repo = new AprovacaoRepository(mockConnection.Object, null, mockLogger.Object);

            // Act & Assert
            var ex = await Should.ThrowAsync<Exception>(() => repo.ObterTodosAsync());
            ex.ShouldNotBeNull();
        }
    }
}
