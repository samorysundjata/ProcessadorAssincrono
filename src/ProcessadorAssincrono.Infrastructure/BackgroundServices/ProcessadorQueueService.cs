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


        public ProcessadorQueueService(Channel<Aprovacao> channel, IServiceProvider serviceProvider, ILogger<ProcessadorQueueService> logger)
        {
            _channel = channel;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task EnfileirarAsync(Guid id, string projeto, string comentariosAdicionais, DateTime dataAprovacao)
        {
            var mensagem = new Aprovacao { Id = id, Projeto = projeto, ComentariosAdicionais = comentariosAdicionais, DataAprovacao = dataAprovacao };
            await _channel.Writer.WriteAsync(mensagem);
            _logger.LogInformation($"Enfileirado: {id}");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(attempt),
                    (ex, ts) => _logger.LogWarning($"Erro: {ex.Message}. Tentando novamente..."));

            while (await _channel.Reader.WaitToReadAsync(stoppingToken))
            {
                var mensagem = await _channel.Reader.ReadAsync(stoppingToken);
                _logger.LogInformation($"Lendo a solicitação: {mensagem.Id}");

                await retryPolicy.ExecuteAsync(async () =>
                {
                    try
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var service = scope.ServiceProvider.GetRequiredService<IAprovacaoService>();

                        await service.AprovarAsync(mensagem.Id, mensagem.Projeto, mensagem.ComentariosAdicionais, mensagem.DataAprovacao);
                        _logger.LogInformation($"Processado a mensagem: {mensagem.Id}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Falha ao processar a mensagem {mensagem.Id}: {ex.Message}");
                        throw;
                    }
                });
            }
        }
    }
}
