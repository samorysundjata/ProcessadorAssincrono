using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using ProcessadorAssincrono.Application.Interfaces;
using ProcessadorAssincrono.Domain.Entities;
using System.Threading.Channels;

namespace ProcessadorAssincrono.Infrastructure.BackgroundServices
{
    public class ProcessadorQueueService : BackgroundService, IProcessadorQueue
    {
        private readonly Channel<Aprovacao> _channel;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ProcessadorQueueService> _logger;
        private readonly CancellationToken _cancellationToken;


        public ProcessadorQueueService(Channel<Aprovacao> channel, IServiceProvider serviceProvider, ILogger<ProcessadorQueueService> logger, CancellationToken cancellationToken)
        {
            _channel = channel;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _cancellationToken = cancellationToken;
        }

        public async Task EnfileirarAsync(Guid id, string projeto, string comentariosAdicionais, DateTime dataAprovacao)
        {
            var mensagem = new Aprovacao { Id = id, Projeto = projeto, ComentariosAdicionais = comentariosAdicionais, DataAprovacao = dataAprovacao };
            await _channel.Writer.WriteAsync(mensagem, _cancellationToken);
            _logger.LogInformation("Enfileirado: {AprovacaoId}", id);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(attempt),
                    (ex, ts) => _logger.LogWarning(ex, "Erro ao processar. Tentando novamente..."));

            while (await _channel.Reader.WaitToReadAsync(stoppingToken))
            {
                var mensagem = await _channel.Reader.ReadAsync(stoppingToken);
                _logger.LogInformation("Lendo a solicitação: {AprovacaoId}", mensagem.Id);

                await retryPolicy.ExecuteAsync(async () =>
                {
                    try
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var service = scope.ServiceProvider.GetRequiredService<IAprovacaoService>();

                        await service.AprovarAsync(mensagem.Id, mensagem.Projeto, mensagem.ComentariosAdicionais, mensagem.DataAprovacao);
                        _logger.LogInformation("Processado a mensagem: {AprovacaoId}", mensagem.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Falha ao processar a mensagem {AprovacaoId}", mensagem.Id);
                        throw;
                    }
                });
            }
        }
    }
}
