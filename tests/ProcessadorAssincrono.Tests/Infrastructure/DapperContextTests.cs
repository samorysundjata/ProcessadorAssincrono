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
            var mockSection = new Mock<IConfigurationSection>();
            mockSection.Setup(s => s["DefaultConnection"]).Returns((string?)null);
            mockConfig.Setup(c => c.GetSection("ConnectionStrings")).Returns(mockSection.Object);           

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => new DapperContext(mockConfig.Object));
            Assert.Equal("Connection string not configured.", exception.Message);
        }

        [Fact(DisplayName = "Construtor deve lançar exceção se a connection string for vazia")]
        public void Constructor_ShouldThrow_WhenConnectionStringIsEmpty()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockSection = new Mock<IConfigurationSection>();
            mockSection.Setup(s => s["DefaultConnection"]).Returns(string.Empty);
            mockConfig.Setup(c => c.GetSection("ConnectionStrings")).Returns(mockSection.Object);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => new DapperContext(mockConfig.Object));
            Assert.Equal("Connection string not configured.", exception.Message);
        }
    }
}
