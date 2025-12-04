using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using ProcessadorAssincrono.Application.Interfaces;
using ProcessadorAssincrono.Domain.Entities;
using ProcessadorAssincrono.Infrastructure.BackgroundServices;
using System.Threading.Channels;

namespace ProcessadorAssincrono.Tests.Infrastructure
{
    public class ProcessadorQueueServiceTests
    {
        private readonly Channel<Aprovacao> _channel;
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<ILogger<ProcessadorQueueService>> _loggerMock;
        private readonly Mock<IAprovacaoService> _aprovacaoServiceMock;
        private readonly ProcessadorQueueService _service;

        public ProcessadorQueueServiceTests()
        {
            _channel = Channel.CreateUnbounded<Aprovacao>();
            _serviceProviderMock = new Mock<IServiceProvider>();
            _loggerMock = new Mock<ILogger<ProcessadorQueueService>>();
            _aprovacaoServiceMock = new Mock<IAprovacaoService>();

            var scopeMock = new Mock<IServiceScope>();
            var scopeFactoryMock = new Mock<IServiceScopeFactory>();

            scopeMock.Setup(s => s.ServiceProvider).Returns(() =>
            {
                var spMock = new Mock<IServiceProvider>();
                spMock.Setup(sp => sp.GetService(typeof(IAprovacaoService)))
                      .Returns(_aprovacaoServiceMock.Object);
                return spMock.Object;
            });

            scopeFactoryMock.Setup(sf => sf.CreateScope()).Returns(scopeMock.Object);
            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IServiceScopeFactory)))
                                 .Returns(scopeFactoryMock.Object);

            _service = new ProcessadorQueueService(_channel, _serviceProviderMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task EnfileirarAsync_Deve_Adicionar_Item_No_Canal_E_Logar()
        {
            // Arrange
            var id = Guid.NewGuid();
            var pep = "PEP123";
            var comentarios = "Teste";
            var data = DateTime.Now;

            // Act
            await _service.EnfileirarAsync(id, pep, comentarios, data);

            // Assert
            Assert.True(await _channel.Reader.WaitToReadAsync());
            _loggerMock.Verify(l => l.Log(
                It.Is<LogLevel>(lvl => lvl == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Enfileirado")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_Deve_Processar_Item_E_Chamar_AprovacaoService()
        {
            // Arrange
            var id = Guid.NewGuid();
            var pep = "PEP123";
            var comentarios = "Teste";
            var data = DateTime.Now;

            await _channel.Writer.WriteAsync(new Aprovacao { Id = id, Pep = pep, ComentariosAdicionais = comentarios, DataAprovacao = data });

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(2)); // Para não travar o teste

            // Act
            await _service.StartAsync(cts.Token); // Inicia o BackgroundService
            await Task.Delay(500); // Aguarda processamento

            // Assert
            _aprovacaoServiceMock.Verify(s => s.AprovarAsync(id, pep, comentarios, data), Times.Once);
            _loggerMock.Verify(l => l.Log(
                It.Is<LogLevel>(lvl => lvl == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Processado")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.AtLeastOnce);
        }
    }
}
