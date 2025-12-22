using Dapper;
using Microsoft.Extensions.Logging;
using ProcessadorAssincrono.Domain.Entities;
using ProcessadorAssincrono.Infrastructure.Interfaces;
using System.Data;

namespace ProcessadorAssincrono.Infrastructure.Persistence
{
    public class AprovacaoRepository : IAprovacaoRepository
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction? _transaction;
        private readonly ILogger<AprovacaoRepository> _logger;

        public AprovacaoRepository(IDbConnection connection, IDbTransaction? transaction, ILogger<AprovacaoRepository> logger)
        {
            _connection = connection;
            _transaction = transaction;
            _logger = logger;
        }

        public Task InserirAsync(Aprovacao entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            if (entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();

            const string sql = "INSERT INTO Aprovacoes (Id, Pep, ComentariosAdicionais, DataAprovacao) VALUES (@Id, @Pep, @ComentariosAdicionais, @DataAprovacao);";

            return _connection.ExecuteAsync(sql, new
            {
                entity.Id,
                entity.Projeto,
                entity.ComentariosAdicionais,
                entity.DataAprovacao
            }, _transaction);
        }

        public async Task<Aprovacao?> ObterPorId(Guid id)
        {
            const string sql = "SELECT Id, Pep, ComentariosAdicionais, DataAprovacao FROM Aprovacoes WHERE Id = @Id;";
            try
            {
                if (_connection is System.Data.Common.DbConnection dbConn && dbConn.State != ConnectionState.Open)
                {
                    await dbConn.OpenAsync();
                }
                else if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }

                return await _connection.QuerySingleOrDefaultAsync<Aprovacao>(sql, new { Id = id }, _transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter aprovação por Id {AprovacaoId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Aprovacao>> ObterTodosAsync()
        {
            const string sql = "SELECT Id, Pep, ComentariosAdicionais, DataAprovacao FROM Aprovacoes;";
            try
            {
                if (_connection is System.Data.Common.DbConnection dbConn && dbConn.State != ConnectionState.Open)
                {
                    await dbConn.OpenAsync();
                }
                else if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }

                return await _connection.QueryAsync<Aprovacao>(sql, transaction: _transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter todas as aprovações");
                throw;
            }
        }
    }
}
