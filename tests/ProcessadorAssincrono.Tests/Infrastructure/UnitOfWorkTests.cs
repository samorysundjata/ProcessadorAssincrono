using Microsoft.Extensions.Logging;
using Moq;
using ProcessadorAssincrono.Infrastructure.Interfaces;
using ProcessadorAssincrono.Infrastructure.Persistence;
using Shouldly;
using System.Data;

namespace ProcessadorAssincrono.Tests.Infrastructure
{
    public class UnitOfWorkTests
    {
        [Fact(DisplayName = "Construtor deve abrir conexão, iniciar transação e instanciar Aprovacoes")]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // Arrange
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(c => c.BeginTransaction()).Returns(Mock.Of<IDbTransaction>());

            var mockFactory = new Mock<IDbConnectionFactory>();
            mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);

            var mockLoggerFactory = new Mock<ILoggerFactory>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                             .Returns(Mock.Of<ILogger>());

            // Act
            var uow = new UnitOfWork(mockFactory.Object, mockLoggerFactory.Object);

            // Assert
            uow.ShouldNotBeNull();
            uow.Aprovacoes.ShouldNotBeNull();
            mockConnection.Verify(c => c.Open(), Times.Once);
            mockConnection.Verify(c => c.BeginTransaction(), Times.Once);
        }

        [Fact(DisplayName = "CommitAsync deve confirmar transação e fechar conexão")]
        public async Task CommitAsync_ShouldCommitAndCloseConnection()
        {
            // Arrange
            var mockTransaction = new Mock<IDbTransaction>();
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(c => c.BeginTransaction()).Returns(mockTransaction.Object);

            var mockFactory = new Mock<IDbConnectionFactory>();
            mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);

            var mockLoggerFactory = new Mock<ILoggerFactory>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                             .Returns(Mock.Of<ILogger>());

            var uow = new UnitOfWork(mockFactory.Object, mockLoggerFactory.Object);

            // Act
            await uow.CommitAsync();

            // Assert
            mockTransaction.Verify(t => t.Commit(), Times.Once);
            mockTransaction.Verify(t => t.Dispose(), Times.Once);
            mockConnection.Verify(c => c.Close(), Times.Once);
            mockConnection.Verify(c => c.Dispose(), Times.Once);
        }

        [Fact(DisplayName = "RollbackAsync deve reverter transação e fechar conexão")]
        public async Task RollbackAsync_ShouldRollbackAndCloseConnection()
        {
            // Arrange
            var mockTransaction = new Mock<IDbTransaction>();
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(c => c.BeginTransaction()).Returns(mockTransaction.Object);

            var mockFactory = new Mock<IDbConnectionFactory>();
            mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);

            var mockLoggerFactory = new Mock<ILoggerFactory>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                             .Returns(Mock.Of<ILogger>());

            var uow = new UnitOfWork(mockFactory.Object, mockLoggerFactory.Object);

            // Act
            await uow.RollbackAsync();

            // Assert
            mockTransaction.Verify(t => t.Rollback(), Times.Once);
            mockTransaction.Verify(t => t.Dispose(), Times.Once);
            mockConnection.Verify(c => c.Close(), Times.Once);
            mockConnection.Verify(c => c.Dispose(), Times.Once);
        }

        [Fact(DisplayName = "Dispose deve descartar conexão e transação")]
        public void Dispose_ShouldDisposeResources()
        {
            // Arrange
            var mockTransaction = new Mock<IDbTransaction>();
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(c => c.BeginTransaction()).Returns(mockTransaction.Object);

            var mockFactory = new Mock<IDbConnectionFactory>();
            mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);

            var mockLoggerFactory = new Mock<ILoggerFactory>();
            mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                             .Returns(Mock.Of<ILogger>());

            var uow = new UnitOfWork(mockFactory.Object, mockLoggerFactory.Object);

            // Act
            uow.Dispose();

            // Assert
            mockTransaction.Verify(t => t.Dispose(), Times.Once);
            mockConnection.Verify(c => c.Dispose(), Times.Once);
        }
    }
}
