using Microsoft.Extensions.Configuration;
using Moq;
using ProcessadorAssincrono.Infrastructure.Data;

namespace ProcessadorAssincrono.Tests.Infrastructure
{
    public class DapperContextConstructorTests
    {
        [Fact(DisplayName = "Construtor deve lançar exceção se a connection string for nula")]
        public void Constructor_ShouldThrow_WhenConnectionStringIsNull()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["ConnectionStrings:DefaultConnection"]).Returns((string?)null);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => new DapperContext(mockConfig.Object));
            Assert.Equal("Connection string not configured.", exception.Message);
        }

        [Fact(DisplayName = "Construtor deve lançar exceção se a connection string for vazia")]
        public void Constructor_ShouldThrow_WhenConnectionStringIsEmpty()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["ConnectionStrings:DefaultConnection"]).Returns(string.Empty);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => new DapperContext(mockConfig.Object));
            Assert.Equal("Connection string not configured.", exception.Message);
        }
    }
}
