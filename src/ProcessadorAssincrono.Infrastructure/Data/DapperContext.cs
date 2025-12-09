using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using ProcessadorAssincrono.Infrastructure.Interfaces;
using System.Data;


namespace ProcessadorAssincrono.Infrastructure.Data
{
    public sealed class DapperContext : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            var conn = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(conn))
                throw new InvalidOperationException("Connection string not configured.");

            _connectionString = conn;
        }

        public IDbConnection CreateConnection() =>
            new SqlConnection(_connectionString);
    }
}
