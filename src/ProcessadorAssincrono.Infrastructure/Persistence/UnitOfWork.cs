using Microsoft.Extensions.Logging;
using ProcessadorAssincrono.Infrastructure.Interfaces;
using System.Data;

namespace ProcessadorAssincrono.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;
        private readonly ILogger<UnitOfWork> _logger;

        public IAprovacaoRepository Aprovacoes { get; }

        public UnitOfWork(IDbConnectionFactory connectionFactory, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<UnitOfWork>();

            _connection = connectionFactory.CreateConnection();
            _connection.Open();
            _transaction = _connection.BeginTransaction();

            Aprovacoes = new AprovacaoRepository(_connection, _transaction, loggerFactory.CreateLogger<AprovacaoRepository>());
        }

        public async Task CommitAsync()
        {
            try
            {
                _transaction?.Commit();
                _logger.LogInformation("[{Hora}] Transação confirmada com sucesso.", DateTime.Now);
            }
            catch (Exception ex)
            {
                _transaction?.Rollback();
                _logger.LogError(ex, "[{Hora}] Erro ao confirmar transação. Realizado rollback.", DateTime.Now);
                throw;
            }
            finally
            {
                _transaction?.Dispose();
                _connection?.Close();
                _connection?.Dispose();
                _logger.LogInformation("[{Hora}] Conexão com banco de dados encerrada após commit.", DateTime.Now);
            }

            await Task.CompletedTask;
        }

        public async Task RollbackAsync()
        {
            try
            {
                _transaction?.Rollback();
                _logger.LogInformation("[{Hora}] Transação revertida com sucesso.", DateTime.Now);
            }
            finally
            {
                _transaction?.Dispose();
                _connection?.Close();
                _connection?.Dispose();
                _logger.LogInformation("[{Hora}] Conexão com banco de dados encerrada após rollback.", DateTime.Now);
            }

            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }
    }
}
